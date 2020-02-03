using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using mCopernicus.EasyChecker;
using Newtonsoft.Json;

namespace LightsailSwitchIp
{
    static class Program
    {
        public static string modStr = "-m awscli lightsail ";

        static void Main()
        {
            if (!File.Exists("./log.log")) File.Create("./log.log").Close();

            foreach (var item in File.ReadAllLines("svr.txt"))
            {
                var svr = item.Split(',');
                var ipName = svr[0];
                var region = svr[1];
                var ddns = svr[2];

                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("python")
                    {
                        Arguments = "-m awscli",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                p.WaitForExit();

                if (p.ExitCode != 1)
                {
                    p.StartInfo.Arguments = modStr + $"get-static-ip --static-ip-name={ipName} --region={region}";
                    Console.WriteLine("Connecting...");
                    p.Start();
                    p.WaitForExit();
                    Console.Clear();

                    var jMsgOip = JsonConvert.DeserializeObject<dynamic>(p.StandardOutput.ReadToEnd());
                    Console.WriteLine(jMsgOip.staticIp.name);
                    Console.WriteLine(jMsgOip.staticIp.ipAddress);
                    Console.WriteLine(jMsgOip.staticIp.attachedTo);

                    Console.WriteLine("Ping...");
                    List<int> pingInts = MPing.Ping(jMsgOip.staticIp.ipAddress.ToString());
                    List<int> tcPingInts = MPing.Tcping(jMsgOip.staticIp.ipAddress.ToString(), 22);
                    if (pingInts.Max() == 0 || tcPingInts.Max() == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Inaccessible:" + jMsgOip.staticIp.ipAddress);
                        Console.WriteLine("Switching IP...");
                        File.WriteAllText("./log.log",
                            File.ReadAllText("./log.log") + DateTime.Now + ":Accessible " + jMsgOip.staticIp.ipAddress + Environment.NewLine);

                        Switchip(jMsgOip.staticIp.name.ToString(), region, jMsgOip.staticIp.isAttached.ToString());
                        Thread.Sleep(500);
                        p.Start();
                        p.WaitForExit();
                        jMsgOip = JsonConvert.DeserializeObject<dynamic>(p.StandardOutput.ReadToEnd());
                        Console.WriteLine("NewIP:" + jMsgOip.staticIp.ipAddress);
                        using (var pStart = new Process
                        {
                            StartInfo = new ProcessStartInfo("curl")
                            { Arguments = ddns.Replace("0.0.0.0", jMsgOip.staticIp.ipAddress.ToString()) }
                        })
                        {
                            pStart.Start();
                            pStart.WaitForExit();
                        }
                        Console.WriteLine("OK");
                        File.WriteAllText("./log.log",
                            File.ReadAllText("./log.log") + DateTime.Now + ":Accessible " + jMsgOip.staticIp.ipAddress + Environment.NewLine);
                    }
                    else
                    {
                        File.WriteAllText("./log.log",
                            File.ReadAllText("./log.log") + DateTime.Now + ":Accessible " + jMsgOip.staticIp.ipAddress + Environment.NewLine);
                        Console.WriteLine("Accessible");
                        Console.WriteLine();
                        using (var pStart = new Process
                        {
                            StartInfo = new ProcessStartInfo("curl")
                            { Arguments = ddns.Replace("0.0.0.0", jMsgOip.staticIp.ipAddress.ToString()) }
                        })
                        {
                            pStart.Start();
                            pStart.WaitForExit();
                        }
                        Console.WriteLine();
                        Console.WriteLine("OK");
                    }
                }
                else
                {
                    Console.WriteLine("maybe awscli not installed");
                    Console.WriteLine("please pip install awscli and try again");
                }

                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        public static void Switchip(string ipName, string regionName, string attachedTo)
        {
            using (var pStart = new Process
            {
                StartInfo = new ProcessStartInfo("python")
                {
                    Arguments =
                        modStr +
                        $"release-static-ip --static-ip-name {ipName} --region={regionName}"
                }
            })
            {
                pStart.Start();
                pStart.WaitForExit();
            }

            Console.WriteLine("release static ip");

            using (var pStart = new Process
            {
                StartInfo = new ProcessStartInfo("python")
                {
                    Arguments =
                        modStr +
                        $"allocate-static-ip --static-ip-name {ipName} --region={regionName}"
                }
            })
            {
                pStart.Start();
                pStart.WaitForExit();
            }

            Console.WriteLine("allocate static ip");

            using (var pStart = new Process
            {
                StartInfo = new ProcessStartInfo("python")
                {
                    Arguments = modStr + $"attach-static-ip --static-ip-name {ipName}" +
                                $" --instance-name {attachedTo} --region={regionName}"
                }
            })
            {
                pStart.Start();
                pStart.WaitForExit();
            }

            Console.WriteLine("attached");

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("python")
                {
                    Arguments = modStr + "get-static-ip --static-ip-name={ipName} --region={region}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            p.Start();
            p.WaitForExit();
            var jMsgOip = JsonConvert.DeserializeObject<dynamic>(p.StandardOutput.ReadToEnd());
            List<int> pingInts = MPing.Ping(jMsgOip.staticIp.ipAddress.ToString());
            List<int> tcPingInts = MPing.Tcping(jMsgOip.staticIp.ipAddress.ToString(), 22);
            if (pingInts.Max() == 0 || tcPingInts.Max() == 0) Switchip(ipName, regionName, attachedTo);
        }
    }
}
