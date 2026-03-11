using System.Collections.Concurrent;
using System.Net;
using ARSoft.Tools.Net.Dns;

namespace OpFwder
{
    public class DnsForwarder : IDisposable
    {
        private readonly IPAddress upstreamDns;
        private readonly int upstreamPort = 53;
        private readonly DnsServer dnsServer;
        private readonly ConcurrentDictionary<(DnsQuestion, IPAddress), CacheEntry> cache = new();
        private readonly Timer cleanupTimer;
        private readonly TimeSpan cleanupInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan staleThreshold = TimeSpan.FromHours(12);

        private readonly int MinTTL = 120;
        private readonly int MaxTTL = 86400;


        private class CacheEntry(DnsMessage responseData, DateTime expiryTime)
        {
            public DnsMessage ResponseData { get; } = responseData;
            public DateTime ExpiryTime { get; } = expiryTime; // 过期时间（基于TTL）
        }

        /// <summary>
        /// 初始化转发器
        /// </summary>
        /// <param name="listenEndPoint">本地监听端点</param>
        /// <param name="upstreamDnsAddress">上游DNS服务器地址</param>
        public DnsForwarder(IPEndPoint listenEndPoint, IPAddress upstreamDnsAddress)
        {
            upstreamDns = upstreamDnsAddress;
            dnsServer = new DnsServer(
                new UdpServerTransport(listenEndPoint),
                new TcpServerTransport(listenEndPoint));
            dnsServer.QueryReceived += OnQueryReceived;

            // 启动定时清理
            cleanupTimer = new Timer(CleanupCache, null, cleanupInterval, cleanupInterval);
        }

        public void Start()
        {
            dnsServer.Start();
        }

        public void Stop()
        {
            dnsServer.Stop();
        }

        private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            if (e.Query is not DnsMessage query) return;
            if (query.Questions.Count == 0)
            {
                e.Response = CreateErrorResponse(query, ReturnCode.ServerFailure);
                return;
            }

            var question = query.Questions[0]; 

            if (cache.TryGetValue((question,GetIpFromDns(query)), out var entry))
            {
                var cachedMessage = entry.ResponseData;
                var response =
                    BuildResponseFromTemplate(query, cachedMessage,
                        (int) (entry.ExpiryTime - DateTime.UtcNow).TotalSeconds);

                if (DateTime.UtcNow <= entry.ExpiryTime)
                {
                    e.Response = response;
                    return;
                }
                e.Response = response;
                _ = Task.Run(() => RefreshCacheAsync(query));
            }
            else
            {
                try
                {
                    var upstreamResponse = await QueryUpstreamAsync(query);
                    if (upstreamResponse != null)
                    {
                        cache[(question, GetIpFromDns(query))] = new CacheEntry(upstreamResponse,
                            DateTime.UtcNow.AddSeconds(GetTtl(upstreamResponse)));

                        e.Response = upstreamResponse; 
                    }
                    else
                    {
                        e.Response = CreateErrorResponse(query, ReturnCode.ServerFailure);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"E: {ex.Message}");
                    e.Response = CreateErrorResponse(query, ReturnCode.ServerFailure);
                }
            }
        }

        private async Task RefreshCacheAsync(DnsMessage originalQuery)
        {
            try
            {
                var question = originalQuery.Questions[0];
                var newResponse = await QueryUpstreamAsync(originalQuery);
                if (newResponse != null)
                {
                    cache[(question, GetIpFromDns(originalQuery))] = new CacheEntry(newResponse,
                        DateTime.UtcNow.AddSeconds(GetTtl(newResponse)));
                    Console.WriteLine($"UP: {question}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E: {ex.Message}");
            }
        }

        private async Task<DnsMessage> QueryUpstreamAsync(DnsMessage query)
        {
            using var dnsClient = new DnsClient([upstreamDns], [new UdpClientTransport(upstreamPort)]);
            return await dnsClient.SendMessageAsync(query);
        }

        private DnsMessage BuildResponseFromTemplate(DnsMessage query, DnsMessage template, int ttl = 0)
        {
            var response = query.CreateResponseInstance();
            response.ReturnCode = template.ReturnCode;
            response.IsAuthoritiveAnswer = template.IsAuthoritiveAnswer;
            response.IsRecursionAllowed = template.IsRecursionAllowed;
            response.IsAuthenticData = template.IsAuthenticData;
            response.IsCheckingDisabled = template.IsCheckingDisabled;

            if (ttl < 30) ttl = 30;

            foreach (var item in template.AnswerRecords.ToList())
            {
                if (item.RecordType == RecordType.A)
                    response.AnswerRecords.Add(new ARecord(item.Name, ttl, ((ARecord) item).Address));
                else if (item.RecordType == RecordType.Aaaa)
                    response.AnswerRecords.Add(new AaaaRecord(item.Name, ttl, ((AaaaRecord) item).Address));
                else
                    response.AnswerRecords.Add(item);
            }

            return response;
        }

        private int GetTtl(DnsMessage message)
        {
            if (!message.AnswerRecords.Any()) return MinTTL;
            var ttl = message.AnswerRecords.Min(x => x.TimeToLive);
            if (ttl < MinTTL) ttl = MinTTL;
            if (ttl > MaxTTL) ttl = MaxTTL;
            return ttl;
        }

        private DnsMessage CreateErrorResponse(DnsMessage query, ReturnCode code)
        {
            var response = query.CreateResponseInstance();
            response.ReturnCode = code;
            return response;
        }

        private void CleanupCache(object state)
        {
            var threshold = DateTime.UtcNow.Add(-staleThreshold);
            var expiredKeys = cache
                .Where(kvp => kvp.Value.ExpiryTime < threshold)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                cache.TryRemove(key, out _);
            }

            Console.WriteLine($"C：{expiredKeys.Count} / {cache.Count}");
        }

        public void Dispose()
        {
            dnsServer?.Stop();
            cleanupTimer?.Dispose();
        }

        private static IPAddress GetIpFromDns(DnsMessage dnsMsg)
        {
            try
            {
                if (dnsMsg is { IsEDnsEnabled: false }) return IPAddress.Any;
                foreach (var eDnsOptionBase in dnsMsg.EDnsOptions.Options.ToList())
                {
                    if (eDnsOptionBase is ClientSubnetOption option)
                        return option.Address;
                }

                return IPAddress.Any;
            }
            catch (Exception)
            {
                return IPAddress.Any;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IPAddress upstream = IPAddress.Parse("119.29.29.29");     
            IPEndPoint listenOn = new IPEndPoint(IPAddress.Any, 53);

            if (PortIsUse(53)) listenOn.Port = 6653;

            var forwarder = new DnsForwarder(listenOn, upstream);
            forwarder.Start();

            Console.WriteLine("按 'q' 键退出...");
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).KeyChar == 'q')
                    break;
                Thread.Sleep(100);
            }

            forwarder.Stop();
            forwarder.Dispose();
        }

        static bool PortIsUse(int port)
        {
            try
            {
                var tcpListeners = System.Net.NetworkInformation.IPGlobalProperties
                    .GetIPGlobalProperties().GetActiveTcpListeners();
                var udpListeners = System.Net.NetworkInformation.IPGlobalProperties
                    .GetIPGlobalProperties().GetActiveUdpListeners();
                return tcpListeners.Any(ep => ep.Port == port) ||
                       udpListeners.Any(ep => ep.Port == port);
            }
            catch
            {
                return true;
            }
        }
    }
}
