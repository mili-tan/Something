using System;
using System.Net;
using System.Text;

namespace InfuraUpload
{
    class Program
    {
        static void Main()
        {
            var webClient = new WebClient {Proxy = new WebProxy("127.0.0.1", 7890)};
            var str = webClient.UploadFile("https://ipfs.infura.io:5001/api/v0/add?pin=false","./test.txt");
            //webClient.Headers.Set("Content-Type", "multipart/form-data");
            //webClient.Headers.Set("application", "x-www-form-urlencoded");
            Console.WriteLine(Encoding.UTF8.GetString(str));
            Console.ReadKey();
        }
    }
}
