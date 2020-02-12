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
            for (int i = 0; i < 4; i++)
            {
                Socket socks = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    { Blocking = true, ReceiveTimeout = 1000, SendTimeout = 1000 };
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
                    var result = socks.BeginConnect(point, null, null);
                    if (!result.AsyncWaitHandle.WaitOne(1500, true)) continue;
                }
                catch
                {
                    continue;
                }
                stopWatch.Stop();
                times.Add(Convert.ToInt32(stopWatch.Elapsed.TotalMilliseconds));
                socks.Close();
                Thread.Sleep(50);
            }
            if (times.Count == 0) times.Add(0);
            return times;
        }

        public static List<int> Ping(string ipStr)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            byte[] bufferBytes = Encoding.Default.GetBytes("abcdefghijklmnopqrstuvwabcdefghi");

            var times = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                times.Add(Convert.ToInt32(ping.Send(ipStr, 120, bufferBytes).RoundtripTime));
                Thread.Sleep(50);
            }

            return times;
        }

        public static List<int> Curl(string urlStr)
        {
            var webClient = new MyCurl.MWebClient() { TimeOut = 1000 };
            var times = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                try
                {
                    webClient.DownloadString(urlStr);
                }
                catch
                {
                    continue;
                }
                stopWatch.Stop();
                times.Add(Convert.ToInt32(stopWatch.Elapsed.TotalMilliseconds));
                Thread.Sleep(50);
            }
            if (times.Count == 0) times.Add(0);
            return times;
        }
    }
}
