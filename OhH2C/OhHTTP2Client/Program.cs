using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace OhHTTP2Client
{
    static class Program
    {
        static void Main()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
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

                var response = http2Client.GetStringAsync("http://www.baidu.com/").Result;
                Console.WriteLine(response);
            }

            stopWatch.Stop();
            Console.WriteLine("______________1_______________");
            Console.WriteLine(stopWatch.Elapsed.TotalMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            Console.WriteLine(new HttpClient(new Http2Handler()).GetStringAsync("https://www.baidu.com/").Result);

            stopWatch.Stop();
            Console.WriteLine("______________2_______________");
            Console.WriteLine(stopWatch.Elapsed.TotalMilliseconds);

            Console.ReadKey();
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
