using System;
using System.Net;
using System.Net.Sockets;
using NATUPNPLib;

namespace MUPnPTest
{
    static class Program
    {
        static void Main()
        {
            var upnpnat = new UPnPNAT();
            var mappings = upnpnat.StaticPortMappingCollection;

            Console.WriteLine(GetLocIp());

            if (mappings != null)
                Console.WriteLine("没有检测到路由器，或者路由器不支持 UPnP 功能。");
            else
                //mappings.Add(1080, "TCP", 10800, GetLocIp(), true, "mUPnPTest");

            Console.ReadKey();
        }

        public static string GetLocIp()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect("www.sjtu.edu.cn", 443);

                return ((IPEndPoint) tcpClient.Client.LocalEndPoint).Address.ToString();
            }

        }
    }
}
