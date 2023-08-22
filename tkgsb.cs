using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

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
                var name = httpContext.Request.Query["name"];
                if (MemoryCache.Default.Contains("T:" + name))
                    return MemoryCache.Default.Get("T:" + name).ToString();

                var httpClient = clientFactory.CreateClient("T");
                httpClient.DefaultRequestHeaders.Add("Client-Ip", "192.168.1.100");
                httpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.1.100");
                httpClient.DefaultRequestHeaders.Add("Referer", "https://../");

                var fullUrl = "https://.../index.php?m=check&a=check&url=" + name;
                var results = await (await httpClient.PostAsync(fullUrl, null)).Content.ReadAsStringAsync();

                var json = results.Substring(1, results.Length - 2);

                Task.Run(() =>
                {
                    if (!MemoryCache.Default.Contains("T:" + name))
                        MemoryCache.Default.Add(
                            new CacheItem("T:" + name, json),
                            new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)});
                });

                httpContext.Response.ContentType = "application/json;charset=UTF-8";
                return json;
            });

            app.MapGet("/k", async (HttpContext httpContext, IHttpClientFactory clientFactory) =>
            {
                var name = httpContext.Request.Query["name"];
                if (MemoryCache.Default.Contains("K:" + name))
                    return MemoryCache.Default.Get("K:" + name).ToString();

                var appKey = "k-";
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
                            new CacheItem("K:" + name, result),
                            new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)});
                });

                httpContext.Response.ContentType = "application/json;charset=UTF-8";
                return result;
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
    }
}