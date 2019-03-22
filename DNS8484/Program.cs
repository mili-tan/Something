using System;
using System.Net;
using MyDnsPackage;

namespace DNS8484
{
    class Program
    {

        static void Main()
        {
            var dnsBase64String = Convert.ToBase64String(GetQuestionData("mili.one", QueryType.A, null));
            var dns = new WebClient().DownloadData("https://dns.rubyfish.cn/dns-query?dns=" + dnsBase64String);
            Console.ReadKey();
        }

        public static byte[] GetQuestionData(string host, QueryType? qtype, byte[] id)
        {
            byte[] mId = id ?? newid();
            QueryType mQtype = qtype ?? QueryType.A;

            var header = new MyDnsHeader();
            header.NewID(mId);
            var question = new MyDnsQuestion();
            question.Class = QueryClass.IN;
            question.Type = mQtype;
            question.Qname = host;
            byte[] dataHead = header.GetBytes();
            byte[] dataQuestion = question.GetBytes();
            byte[] sendData = new byte[dataHead.Length + dataQuestion.Length];
            dataHead.CopyTo(sendData, 0);
            dataQuestion.CopyTo(sendData, dataHead.Length);

            return sendData;
        }

        private static byte[] newid()
        {
            byte[] result = new byte[2];
            Random rd = new Random();
            rd.NextBytes(result);
            return result;
        }
    }
}
