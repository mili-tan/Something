using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using EasyChecker;

namespace tunping
{
    static class Program
    {
        static void Main()
        {
            string myIP = new WebClient().DownloadString("https://api.ip.la");
            string myLoc = new WebClient().DownloadString($"https://freeapi.ipip.net/{myIP}");
            Timer timer = new Timer(1000 * 60 * 10){ AutoReset = true};
            timer.Elapsed += (sender, eventArgs) =>
            {
                var list = File.ReadAllLines("./list.list");
                foreach (var item in list)
                {
                    if (Ping.Tcping(item, 80).Max() == 0)
                        new WebClient().DownloadString($"http://hostname/@chat/{myLoc}:{item} Boom!");
                }
            };
        }
    }
}
