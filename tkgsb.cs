using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace TenKGSB
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.MapGet("/t", async (HttpContext httpContext, IHttpClientFactory clientFactory) =>
            {
                httpContext.Response.ContentType = "application/json;charset=UTF-8";

                var name = httpContext.Request.Query["name"];
                if (MemoryCache.Default.Contains("T:" + name))
                    return Decompress((byte[]) MemoryCache.Default.Get("T:" + name));

                var httpClient = clientFactory.CreateClient("T");
                httpClient.DefaultRequestHeaders.Add("Client-Ip", "192.168.1.100");
                httpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.1.100");
                httpClient.DefaultRequestHeaders.Add("Referer", "https://.../");

                var fullUrl = "https://.../index.php?m=url&a=validUrl&url=http://" + name;
                var json =  (await (await httpClient.PostAsync(fullUrl, null)).Content.ReadAsStringAsync()).Trim();

                Task.Run(() =>
                {
                    if (!MemoryCache.Default.Contains("T:" + name))
                        MemoryCache.Default.Add(
                            new CacheItem("T:" + name, Compress(json)),
                            new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)});
                });

                return json;
            });

            app.MapGet("/k", async (HttpContext httpContext, IHttpClientFactory clientFactory) =>
            {
                httpContext.Response.ContentType = "application/json;charset=UTF-8";

                var name = httpContext.Request.Query["name"];
                if (MemoryCache.Default.Contains("K:" + name))
                    return Decompress((byte[]) MemoryCache.Default.Get("K:" + name));

                var appKey = "";
                var secret = "";

                var q = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
                var nowDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var time = DateTimeOffset.FromUnixTimeMilliseconds(nowDate).ToUnixTimeMilliseconds().ToString();
                var timestamp = time.Substring(0, 10) + ".000";

                var sign = GetMd5Hash($"/phish/?appkey={appKey}&q={q}&timestamp={timestamp}{secret}");
                var url = $"http://open.pc120.com/phish/?q={q}&appkey={appKey}&timestamp={timestamp}&sign={sign}";

                var client = clientFactory.CreateClient("K");
                var result = await (await client.GetAsync(url)).Content.ReadAsStringAsync();

                Task.Run(() =>
                {
                    if (!MemoryCache.Default.Contains("K:" + name))
                        MemoryCache.Default.Add(
                            new CacheItem("K:" + name, Compress(result)),
                            new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)});
                });

                return result;
            });

            
            app.MapGet("/r", async (HttpContext httpContext, IHttpClientFactory clientFactory) =>
            {
                var client = clientFactory.CreateClient("R");
                var name = httpContext.Request.Query["name"];
                var res = false;
                var accountId = "";
                var readKey = "";
                var writeKey = "";

                if (MemoryCache.Default.Contains("R:" + name))
                    return (bool) MemoryCache.Default.Get("R:" + name);

                using var hostRequest = new HttpRequestMessage(new HttpMethod("GET"),
                    $"https://api.cloudflare.com/client/v4/accounts/{accountId}/urlscanner/scan?page_hostname=" +
                    name);
                hostRequest.Headers.TryAddWithoutValidation("Authorization", "Bearer " + readKey);
                var hostJson = JObject.Parse(await (await client.SendAsync(hostRequest)).Content.ReadAsStringAsync());

                if (hostJson["result"]["tasks"].Any())
                {
                    var reportId =
                        (hostJson["result"]["tasks"].FirstOrDefault(i =>
                            i["url"].ToString() == $"https://{name}/" || i["url"].ToString() == $"http://{name}/") ??
                        hostJson["result"]["tasks"].FirstOrDefault())["uuid"].ToString();

                    using var reportRequest = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.cloudflare.com/client/v4/accounts/{accountId}/urlscanner/scan/" + reportId);
                    hostRequest.Headers.TryAddWithoutValidation("Authorization", "Bearer " + readKey);
                    var reportJson = JObject.Parse(await (await client.SendAsync(reportRequest)).Content.ReadAsStringAsync());

                    if (reportJson["success"].ToObject<bool>()) return res;
                    var b = reportJson["result"]["scan"]["verdicts"]["overall"]["malicious"];
                    if (!b.HasValues) b = reportJson["result"]["scan"]["meta"]["phishing"];
                    res = Convert.ToBoolean(b);
                    Task.Run(() =>
                    {
                        if (!MemoryCache.Default.Contains("R:" + name))
                            MemoryCache.Default.Add(
                                new CacheItem("R:" + name, res),
                                new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
                    });
                }
                else
                {
                    using var newScanRequest = new HttpRequestMessage(new HttpMethod("POST"), $"https://api.cloudflare.com/client/v4/accounts/{accountId}/urlscanner/scan");
                    newScanRequest.Headers.TryAddWithoutValidation("Authorization", "Bearer " + writeKey);
                    newScanRequest.Content = new StringContent($"{{\"url\": \"{name}\"}}");
                    newScanRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    await client.SendAsync(newScanRequest);

                    Task.Run(() =>
                    {
                        if (!MemoryCache.Default.Contains("R:" + name))
                            MemoryCache.Default.Add(
                                new CacheItem("R:" + name, false),
                                new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)});
                    });
                }

                return res;
            });


            app.Run();
        }

        private static string GetMd5Hash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using var hash = MD5.Create();
            var hashed = hash.ComputeHash(bytes);

            var builder = new StringBuilder(hashed.Length * 2);
            foreach (byte b in hashed)
                builder.Append(b.ToString("X2"));

            return builder.ToString();
        }

        public static byte[] Compress(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using var memoryStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
                brotliStream.Write(bytes, 0, bytes.Length);
            return memoryStream.ToArray();
        }

        public static string Decompress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using (var decompressStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
                decompressStream.CopyTo(outputStream);
            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}

