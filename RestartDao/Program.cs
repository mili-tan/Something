using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RestartDao
{
    class Program
    {
        static void Main(string[] args)
        {
            int tryNum = 0;
            while (true)
            {
                try
                {

                    string[] data = File.ReadAllLines("./appid.txt");
                    string appid = data[0];
                    string key = data[1];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(@"https://openapi.daocloud.io/v1/apps/{0}/actions/restart", appid));
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Headers["Authorization"] = key;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream mResponseStream = response.GetResponseStream();
                    StreamReader mStreamReader = new StreamReader(mResponseStream, Encoding.GetEncoding("utf-8"));
                    string returnString = mStreamReader.ReadToEnd();
                    mStreamReader.Close();
                    mResponseStream.Close();

                    Console.WriteLine(returnString);

                    Thread.Sleep(300000);

                    IPAddress[] IPs = Dns.GetHostAddresses(data[2]);
                    IPEndPoint point = new IPEndPoint(IPs[0], Convert.ToInt32(data[3]));
                    using (Socket Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        Sock.Connect(point);
                        Console.WriteLine("{0} is OK", point);
                        Sock.Close();
                        break;
                    }
                }

                catch (Exception Error)
                {
                    tryNum++;
                    Console.WriteLine(Error.Message.ToString());
                    if (tryNum ==3)
                    {
                        break;
                    }
                }
            }
            Thread.Sleep(1000);
        }
    }
}
