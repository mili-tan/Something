using System;
using System.Net;
using ARSoft.Tools.Net.Dns;
using OhMyDnsPackage;

namespace DNS8484
{
    static class Program
    {

        static void Main()
        {
            var dnsBase64String = Convert.ToBase64String(MyDnsSend.GetQuestionData("dnsp1.mili.one", RecordType.Aaaa)).TrimEnd('=')
                .Replace('+', '-').Replace('/', '_');
            Console.WriteLine(dnsBase64String);
            var dnsMsg =
                DnsMessage.Parse(new WebClient().DownloadData("https://dns.rubyfish.cn/dns-query?dns=" + dnsBase64String));
            foreach (var item in dnsMsg.AnswerRecords)
                Console.WriteLine(item);

            Console.ReadKey();
        }
    }
}
