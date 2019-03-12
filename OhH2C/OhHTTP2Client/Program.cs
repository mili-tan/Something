using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OhHTTP2Client
{
    static class Program
    {
        static void Main()
        {
            using (var http2Client = new HttpClient(new Http2Handler()))
            {
                http2Client.DefaultRequestHeaders.UserAgent.ParseAdd("OhH2/0.1");
                http2Client.DefaultRequestHeaders.Connection.Add("keep-alive");
                http2Client.Timeout = TimeSpan.FromSeconds(2);

//                http2Client.SendAsync(new HttpRequestMessage
//                    {
//                        Method = new HttpMethod("HEAD"),
//                        RequestUri = new Uri("http://baidu.com/")
//                    })
//                    .Result.EnsureSuccessStatusCode();

                var response = http2Client.GetAsync("https://dns.rubyfish.cn/dns-query?name=mili.one").Result;
                Console.WriteLine(response.RequestMessage.ToString());
                Console.WriteLine(response.ToString());
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadKey();
            }
        }

        public class Http2Handler : WinHttpHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                request.Version = new Version("2.0");
                return base.SendAsync(request, cancellationToken);
            }
        }
    
    }
}
