using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using ProxyKit;

namespace S32OllamaRegistry
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddProxy(httpClientBuilder =>
                            httpClientBuilder.ConfigureHttpClient(client =>
                            {
                                client.Timeout = TimeSpan.FromMinutes(5);
                            }));
                    })
                    .ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(80,
                            listenOptions =>
                            {
                                //listenOptions.UseHttps();
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                    })

                    .Configure(app =>
                    {
                        app.Use(async (context, next) =>
                        {
                            Console.WriteLine(context.Request.Path);
                            await next.Invoke();
                        });
                        app.Map("/v2", svr =>
                        {
                            app.UseRouting().UseEndpoints(endpoint =>
                            {
                                endpoint.Map(
                                    "/v2/library/qwen2.5/blobs/sha256:c5396e06af294bd101b30dce59131a76d2b773e76950acc870eda801d3ab0515",
                                    async context =>
                                    {
                                        context.Response.Redirect(
                                            "https://ollama-registry.s3/sha256-c5396e06af294bd101b30dce59131a76d2b773e76950acc870eda801d3ab0515",
                                            false);
                                        context.Response.StatusCode = 307;
                                    });

                                //74a4da8c9fdbcd15bd1f6d01d621410d31c6fc00986f5eb687824e7b93d7a9db
                                //endpoint.Map(
                                //    "/v2/library/qwen2.5/blobs/sha256:74a4da8c9fdbcd15bd1f6d01d621410d31c6fc00986f5eb687824e7b93d7a9db",
                                //    async context =>
                                //    {
                                //        context.Response.Redirect(
                                //            "https://www.modelscope.cn/models/qwen/Qwen2.5-0.5B-Instruct-gguf/resolve/master/qwen2.5-0.5b-instruct-q4_k_m.gguf");
                                //        context.Response.StatusCode = 307;
                                //    });
                                //endpoint.Map(
                                //    "/library/qwen2.5/blobs/sha256:74a4da8c9fdbcd15bd1f6d01d621410d31c6fc00986f5eb687824e7b93d7a9db",
                                //    async context =>
                                //    {
                                //        context.Response.Redirect(
                                //            "https://www.modelscope.cn/models/qwen/Qwen2.5-0.5B-Instruct-gguf/resolve/master/qwen2.5-0.5b-instruct-q4_k_m.gguf");
                                //        context.Response.StatusCode = 307;
                                //    });
                                //endpoint.Map(
                                //    "/v2/library/qwen2.5/blobs/sha256:005f95c7475154a17e84b85cd497949d6dd2a4f9d77c096e3c66e4d9c32acaf5",
                                //    async context =>
                                //    {
                                //        await context.Response.WriteAsync("{\"model_format\":\"gguf\",\"model_family\":\"qwen2\",\"model_families\":[\"qwen2\"],\"model_type\":\"494.03M\",\"file_type\":\"Q4_K_M\",\"architecture\":\"amd64\",\"os\":\"linux\",\"rootfs\":{\"type\":\"layers\",\"diff_ids\":[\"sha256:74a4da8c9fdbcd15bd1f6d01d621410d31c6fc00986f5eb687824e7b93d7a9db\",\"sha256:75357d685f238b6afd7738be9786fdafde641eb6ca9a3be7471939715a68a4de\",\"sha256:9bebd78bf5bc92d41d5f3aab3ee66c891376b4eb4cf433edc2533c2f5f9c95a6\",\"sha256:832dd9e00a68dd83b3c3fb9f5588dad7dcf337a0db50f7d9483f310cd292e92e\"]}}\n");
                                //    });
                            });
                            svr.RunProxy(async context =>
                            {
                                var response = new HttpResponseMessage();
                                try
                                {
                                    context.Request.Headers.Remove("Accept-Encoding");
                                    context.Request.Headers.TryAdd("Accept-Encoding", "identity");
                                    response = await context.ForwardTo(new Uri("https://registry.ollama.ai/v2/")).Send();

                                    response.Headers.Add("X-Forwarder-By", "KENZO/OllamaRegistry");

                                    //if (response.StatusCode != HttpStatusCode.OK)
                                    return response;

                                    //var reResponse = await response.ReplaceContent(async upContent =>
                                    //{
                                    //    var body = await GetBody(upContent);
                                    //    return new StringContent(
                                    //        body.Replace(
                                    //            "c5396e06af294bd101b30dce59131a76d2b773e76950acc870eda801d3ab0515",
                                    //            "74a4da8c9fdbcd15bd1f6d01d621410d31c6fc00986f5eb687824e7b93d7a9db"),
                                    //        Encoding.UTF8, response.Content.Headers.ContentType.MediaType);
                                    //});
                                    //context.Response.RegisterForDispose(response);
                                    //return reResponse;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    return response;
                                }
                            });
                        });
                    }).Build();

                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task<string> GetBody(HttpContent content)
        {
            var bytes = await content.ReadAsByteArrayAsync();
            if ((bytes[0] == 31 && bytes[1] == 139) ||
                (content.Headers.TryGetValues("content-encoding", out var codeValues)
                 && codeValues.Contains("gzip")))
                return new StreamReader(new GZipStream(new MemoryStream(bytes),
                    CompressionMode.Decompress), Encoding.UTF8).ReadToEnd();
            return new StreamReader(new MemoryStream(bytes), Encoding.UTF8).ReadToEnd();
        }
    }
}
