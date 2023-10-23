using System.Text.Json;
using ARSoft.Tools.Net.Dns;

namespace B2D
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Any())
            {
                var decode = Convert.FromBase64String(args[0].Trim('\"'));
                var msg = DnsMessage.Parse(decode);
                Console.WriteLine(JsonSerializer.Serialize(msg));
                Console.WriteLine();
                if (msg.Questions.Any())
                {
                    Console.WriteLine("Questions:");
                    foreach (var item in msg.Questions) Console.WriteLine(item.ToString());
                }
                if (msg.AnswerRecords.Any())
                {
                    Console.WriteLine("AnswerRecords:");
                    foreach (var item in msg.AnswerRecords) Console.WriteLine(item.ToString());
                }
                if (msg.AuthorityRecords.Any())
                {
                    Console.WriteLine("AuthorityRecords:");
                    foreach (var item in msg.AuthorityRecords) Console.WriteLine(item.ToString());
                }
                if (msg.AdditionalRecords.Any())
                {
                    Console.WriteLine("AdditionalRecords:");
                    foreach (var item in msg.AdditionalRecords) Console.WriteLine(item.ToString());
                }
            }
            else
            {
                Console.WriteLine("ArashiDNS.B2D - DNSBase64 Parsing Tool");
                Console.WriteLine($"Copyright (c) {DateTime.Now.Year} Milkey Tan. Code released under the MIT License.");
                Console.WriteLine();
                Console.WriteLine("Not a valid DNSBase64 string.");
            }
        }
    }
}