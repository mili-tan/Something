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

                var fullUrl = "https://.../index.php?m=check&a=check&url=" + name;
                var results = await (await httpClient.PostAsync(fullUrl, null)).Content.ReadAsStringAsync();

                var json = results.Substring(1, results.Length - 2);

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
