using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OhHTTP2Client
{
    static class Program
    {
        static void Main()
        {
            using (var httpClient = new HttpClient(new Http2Handler()))
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OhH2/0.1");
                var response = httpClient.GetAsync("https://dns.rubyfish.cn/dns-query?name=mili.one").Result;
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
