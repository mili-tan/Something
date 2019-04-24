﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EasyChecker
{
    static class Ping
    {
        public static List<int> Tcping(string ip,int port)
        {
            var times = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                Socket socks = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = true
                };

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
                    times.Add(0);
                    return times;
                }
                stopWatch.Stop();

                double time = stopWatch.Elapsed.TotalMilliseconds;
                times.Add(Convert.ToInt32(time));
                socks.Close();

                Thread.Sleep(100);
            }

            return times;
        }

        public static List<int> MPing(string ipStr)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            byte[] bufferBytes = { 00, 01, 00, 01, 00, 01, 00, 01 };

            var times = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                times.Add(Convert.ToInt32(ping.Send(ipStr, 50, bufferBytes).RoundtripTime));
                Thread.Sleep(100);
            }

            return times;
        }
    }
}
