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
            dnsQMsg.Questions.Add(new DnsQuestion(DomainName.Parse("pixiv.net"), RecordType.A, RecordClass.INet));
            dnsQMsg.IsEDnsEnabled = true;
            dnsQMsg.EDnsOptions.Options.Add(new ClientSubnetOption(24, IPAddress.Parse("0.0.0.0")));
            //dnsQMsg.Encode(false, out var bytes);

            var args = new object[] {false, null};
            var mms = dnsQMsg.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var mm in mms)
                if (mm.ToString() == "Int32 Encode(Boolean, Byte[] ByRef)")
                    mm.Invoke(dnsQMsg, args);

            if ((args[1] as byte[])[2] == 0) (args[1] as byte[])[2] = 1;

            //var ibyte = MyDnsSend.GetQuestionData(DomainName.Parse("baidu.com").ToString().TrimEnd('.'), RecordType.A);
            //Console.WriteLine(ibyte);

            var dnsBase64String = Convert.ToBase64String((byte[])args[1]).TrimEnd('=')
                .Replace('+', '-').Replace('/', '_');
            Console.WriteLine(dnsBase64String);
            var dnsMsg =
                DnsMessage.Parse(new WebClient().DownloadData("https://doh1.blahdns.com/dns-query?dns=" + dnsBase64String));
            
            Console.WriteLine(dnsMsg.ReturnCode);
            foreach (var item in dnsMsg.AnswerRecords)
                Console.WriteLine(item);

            Console.ReadKey();
        }
    }
}
