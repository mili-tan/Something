using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using QuicNet;
using QuicNet.Connections;
using QuicNet.Streams;

namespace OvQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            QuicClient qClient = new QuicClient();
            QuicConnection qConnection = qClient.Connect("127.0.0.1", 11000);
            QuicStream qStream = qConnection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);

            Console.ReadKey();
            qStream.Send(Encoding.UTF8.GetBytes("Hello from Client!"));
            Console.WriteLine(qStream.Receive().ToString());
        }
    }
}
