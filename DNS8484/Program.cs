using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using OhMyDnsPackage;

namespace DNS8484
{
    static class Program
    {

        static void Main()
        {
            var dnsQMsg = new DnsMessage();
            dnsQMsg.Questions.Add(new DnsQuestion(DomainName.Parse("google.com"), RecordType.A, RecordClass.Any));
            //dnsQMsg.Encode(false, out var bytes);

            var args = new object[] {false, null};
            var mms = dnsQMsg.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var mm in mms)
                if (mm.ToString() == "Int32 Encode(Boolean, Byte[] ByRef)")
                    mm.Invoke(dnsQMsg, args);

            Console.WriteLine(args.Length);
            //MyDnsSend.GetQuestionData("dnsp1.mili.one", RecordType.Aaaa)
            var dnsBase64String = Convert.ToBase64String((byte[])args[1]).TrimEnd('=')
                .Replace('+', '-').Replace('/', '_');
            Console.WriteLine(dnsBase64String);
            var dnsMsg =
                DnsMessage.Parse(new WebClient().DownloadData("https://223.5.5.5/dns-query?dns=" + dnsBase64String));
            foreach (var item in dnsMsg.AnswerRecords)
                Console.WriteLine(item);

            Console.ReadKey();
        }
    }
}
