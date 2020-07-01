using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using LettuceEncrypt;
using Microsoft.Extensions.Logging;

namespace Lettuce
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .ConfigureLogging(configureLogging => { configureLogging.AddConsole(); })
                .UseContentRoot(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddLettuceEncrypt(configure =>
                    {
                        configure.AcceptTermsOfService = true;
                        configure.DomainNames = new[] {"test.mili.one"};
                        configure.EmailAddress = "me@milione.cc";
                    }).PersistDataToDirectory(new DirectoryInfo("/LettuceEncrypt"), null);
                })
                .UseKestrel(k =>
                {
                    //var appServices = k.ApplicationServices;
                    //k.ConfigureHttpsDefaults(h =>
                    //{
                    //    h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    //    h.UseLettuceEncrypt(appServices);
                    //});
                })
                .ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 1024;
                    options.Listen(new IPEndPoint(IPAddress.Any, 80),
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
