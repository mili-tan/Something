using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using mCopernicus.EasyChecker;
using Newtonsoft.Json;

namespace LightsailSwitchIp
{
    static class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in File.ReadAllLines("svr.txt"))
            {
                var svr = item.Split(",");
                var ipName = svr[0];
                var region = svr[1];
                var ddns = svr[2];

                var modStr = "-m awscli lightsail ";
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("python")
                    {
                        Arguments = "-m awscli", UseShellExecute = false, RedirectStandardOutput = true,
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

                        using (var pStart = new Process
                        {
                            StartInfo = new ProcessStartInfo("python")
                            {
                                Arguments =
                                    modStr +
                                    $"release-static-ip --static-ip-name {jMsgOip.staticIp.name} --region={region}"
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
                                    $"allocate-static-ip --static-ip-name {jMsgOip.staticIp.name} --region={region}"
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
                                Arguments = modStr + $"attach-static-ip --static-ip-name {jMsgOip.staticIp.name}" +
                                            $" --instance-name {jMsgOip.staticIp.attachedTo} --region={region}"
                            }
                        })
                        {
                            pStart.Start();
                            pStart.WaitForExit();
                        }

                        Console.WriteLine("attached");

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
                    }
                    else
                    {
                        Console.WriteLine("Accessible");
                        Console.WriteLine();
                        using (var pStart = new Process
                        {
                            StartInfo = new ProcessStartInfo("curl")
                                {Arguments = ddns.Replace("0.0.0.0", jMsgOip.staticIp.ipAddress.ToString())}
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
    }
}
