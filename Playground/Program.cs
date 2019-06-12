using System;
using System.Net;
using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using SC;
using static System.Net.NetworkInformation.NetworkInterface;
using static System.Net.NetworkInformation.OperationalStatus;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Shortcut link = new Shortcut();
            //link.Load($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\Telegram.lnk");
            link.Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            link.Description = "桌面面面面面面面";
            
            link.Save(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\my.lnk");

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            Console.WriteLine(
                "path = {0}\nargs = {1}\ndirectory = {2}",
                link.Path, link.Arguments, link.WorkingDirectory);

            foreach (var ni in GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == Up)
                    Console.WriteLine("当前正在连接的是：" + ni.Name);
                else
                    Console.WriteLine("当前" + ni.Name + "处于静止或者中断状态。");
            }


            var i = new Uri("https://1.1.1.1/");
            Console.WriteLine(DomainName.Parse("114.114.114.1").ToString());
            Console.WriteLine(i.DnsSafeHost);
            Console.WriteLine(i.Host);
            Console.WriteLine(i.AbsolutePath);
            Console.WriteLine(i.Scheme);
            Console.WriteLine(i.Port);
            Console.WriteLine(new MIpBkWebClient().DownloadString(i));
            Console.ReadKey();
        }

        public class MIpBkWebClient : WebClient
        {
            public bool AllowAutoRedirect { get; set; } = false;
            public int TimeOut { get; set; } = 15000;
            protected override WebRequest GetWebRequest(Uri address)
            {

                var ipAdd = ResolveNameBk(DomainName.Parse(address.DnsSafeHost));
                var mAdd = new Uri(address.Scheme + Uri.SchemeDelimiter + ipAdd + address.AbsolutePath);
                var request = base.GetWebRequest(mAdd);
                request.Timeout = TimeOut;
                if (request is HttpWebRequest webRequest)
                {
                    webRequest.Host = address.DnsSafeHost;
                    webRequest.AllowAutoRedirect = AllowAutoRedirect;
                    webRequest.KeepAlive = true;
                }
                return request;
            }

            private static IPAddress ResolveNameBk(DomainName name)
            {
                if (IPAddress.TryParse(name.ToString().TrimEnd('.'),out _)) return IPAddress.Parse(name.ToString().TrimEnd('.'));
                while (true)
                {
                    var ipMsg = new DnsClient(IPAddress.Parse("1.1.1.1"), 5000).Resolve(name).AnswerRecords[0];
                    if (ipMsg.RecordType == RecordType.A)
                    {
                        if (ipMsg is ARecord msg) return msg.Address;
                    }
                    else
                    {
                        if (ipMsg is CNameRecord msg) name = msg.CanonicalName;
                    }
                }
            }
        }
    }
}
