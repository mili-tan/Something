﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mCopernicus.EasyChecker
{
    static class MPing
    {
        public static List<int> Tcping(string ip,int port)
        {
            var times = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                Socket socks = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    {Blocking = true, ReceiveTimeout = 6000, SendTimeout = 6000};

                IPEndPoint point;
                try
                {
                    point = new IPEndPoint(IPAddress.Parse(ip), port);
                }
                catch
                {
                    point = new IPEndPoint(Dns.GetHostAddresses(ip)[0], port);
                }
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                try
                {
                    socks.Connect(point);
                }
                catch
                {
                    //times.Add(0);
                }
                stopWatch.Stop();
                times.Add(Convert.ToInt32(stopWatch.Elapsed.TotalMilliseconds));
                socks.Close();
                Thread.Sleep(50);
            }

            return times;
        }

        public static List<int> Ping(string ipStr)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            byte[] bufferBytes = Encoding.Default.GetBytes("abcdefghijklmnopqrstuvwabcdefghi");

            var times = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                times.Add(Convert.ToInt32(ping.Send(ipStr, 50, bufferBytes).RoundtripTime));
                Thread.Sleep(50);
            }

            return times;
        }
    }
}
