using System;
using System.Text;
using QuicNet;
using QuicNet.Connections;

namespace OvQ_S
{
    class Program
    {
        static void Main(string[] args)
        {
            QuicListener listener = new QuicListener(11000);
            listener.Start();
            while (true)
            {

                QuicConnection client = listener.AcceptQuicClient();

                client.OnDataReceived += (c) => {
                    byte[] data = c.Data;
                    Console.WriteLine("Data received: " + Encoding.UTF8.GetString(data));
                    c.Send(Encoding.UTF8.GetBytes("Echo!"));
                };
            }
        }
    }
}
