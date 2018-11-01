using System;
using System.IO;
using System.Net;
using System.Threading;

namespace SimpleDownloadDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("请输入下载链接 - {0}代替专辑 {1}代替文件名");
            string url = Console.ReadLine();

            Console.WriteLine("是否使用代理？（y/n） - 仅支持HTTP代理。");
            WebProxy myProxy = null;
            bool proxyEnble = Console.ReadLine() == "y";
            if (proxyEnble)
            {
                Console.Write("请输入代理IP：");
                string proxyIP = Console.ReadLine();
                Console.Write("请输入代理端口：");
                int proxyPort = Convert.ToInt32(Console.ReadLine());
                myProxy = new WebProxy(proxyIP, proxyPort);
            }

            Console.Write("请输入专辑：");
            string album = Console.ReadLine();

            int countNum = 0;
            int errorNum = 0;

            if (!Directory.Exists("./" + album))
            {
                Directory.CreateDirectory("./" + album);
            }

            while (errorNum != 5)
            {
                countNum++;
                string myUrl = string.Format(url, album, countNum.ToString("000"));
                try
                {
                    WebClient myWebClient = new WebClient();
                    if (proxyEnble)
                    {
                        myWebClient.Proxy = myProxy;
                    }
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("正在下载 : " + myUrl);
                    myWebClient.DownloadFile(myUrl, string.Format(@".\{0}\{1}.jpg",album,countNum));
                    Console.WriteLine("完成下载 : " + myUrl);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("发生错误 : " + myUrl + " : " + exp.Message);
                    File.Delete(string.Format(@".\{0}\{1}.jpg", album, countNum));
                    errorNum++;
                }
                Thread.Sleep(100);
            }
            Console.WriteLine("------------已完成------------");
            Console.WriteLine("按任意键结束……");
            Console.ReadKey();
        }
    }
}
