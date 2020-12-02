using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace OnePageSev
{
    class Program
    {
        public static void Main(string[] args)
        {
            var file = File.ReadAllText("index.html");
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
                .ConfigureServices(services => services.AddRouting())
                .ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 1024;
                    options.Listen(new IPEndPoint(IPAddress.Loopback, 2800),
                        listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2);
                })
                .Configure(app =>
                {
                    app.UseStaticFiles();
                    app.UseDefaultFiles();
                    app.UseRouting().UseEndpoints(endpoints =>
                    {
                        endpoints.Map("/", async context =>
                        {
                            context.Response.Headers.Add("X-Powered-By", "DreamLayer.OnePage");
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(file);
                        });
                        endpoints.Map("/{any}", async context =>
                        {
                            context.Response.Headers.Add("X-Powered-By", "DreamLayer.OnePage");
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(file);
                        });
                        endpoints.Map("/{any}/{sth}", async context =>
                        {
                            context.Response.Headers.Add("X-Powered-By", "DreamLayer.OnePage");
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(file);
                        });
                    });
                })
                .Build();
            host.Run();
        }
    }
}
