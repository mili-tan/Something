using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace mThunderLink
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string DownloadLink = "";

            if (!string.IsNullOrWhiteSpace(string.Join("", args)))
            {

                string urlLink = string.Join("", args);
                string[] linkArray = Regex.Split(urlLink, "://");
                linkArray[1] = linkArray[1].Replace("/", "");
                if (linkArray[0].ToLower() == "thunder")
                {
                    string myLink = DeCodeBase64(linkArray[1]);
                    DownloadLink = myLink.Remove(0, 2).Remove(myLink.Count() - 4, 2);
                }
                else if (linkArray[0].ToLower() == "flashget")
                {
                    DownloadLink = DeCodeBase64(linkArray[1]).Replace("[FLASHGET]", "");
                }
                else if (linkArray[0].ToLower() == "qqdl")
                {
                    DownloadLink = DeCodeBase64(linkArray[1]);
                }
                Process.Start("explorer", DownloadLink);
                Console.WriteLine("已解析");
                Console.WriteLine(DownloadLink);
                Clipboard.SetDataObject(DownloadLink.ToString());
                Console.ReadKey();
            }
            else
            {
                Console.Title = "mThunderLink";
                Console.WriteLine("======================");
                Console.WriteLine("mThunderLink");
                Console.WriteLine("======================");
                if (IsAdmin() == false)
                {
                    Console.WriteLine("权限不足，请以管理员权限运行");

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                    startInfo.Verb = "runas";
                    Thread.Sleep(1500);
                    try
                    {
                        Process.Start(startInfo);
                        return;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("======================");
                        Console.WriteLine(e.Message);
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    Console.WriteLine("按任意键注册链接关联，按C取消，按U反注册关联");
                    Console.WriteLine("======================");
                    string keyStr = Console.ReadKey().Key.ToString();
                    if (keyStr == "C")
                    {
                        Console.WriteLine();
                        Console.WriteLine("======================");
                        Console.WriteLine("已取消");
                        Console.WriteLine("======================");
                        Thread.Sleep(1000);
                        return;
                    }
                    else if (keyStr == "U")
                    {
                        try
                        {
                            UnRegURL("thunder");
                            UnRegURL("qqdl");
                            Console.WriteLine();
                            Console.WriteLine("======================");
                            Console.WriteLine("已反注册");
                            Console.WriteLine("======================");
                            Thread.Sleep(1000);
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine();
                            Console.WriteLine("======================");
                            Console.WriteLine(e.Message);
                            Thread.Sleep(1500);
                        }
                    }
                    else
                    {
                        RegURL("thunder");
                        RegURL("qqdl");
                        Console.WriteLine();
                        Console.WriteLine("======================");
                        Console.WriteLine("已注册");
                        Console.WriteLine("======================");
                        Thread.Sleep(1000);
                        return;
                    }
                }
            }
        }
        private static string DeCodeBase64(string Str)
        {
            byte[] Bytes = Convert.FromBase64String(Str);
            string DeCode = Encoding.UTF8.GetString(Bytes);
            return DeCode;
        }
        private static void RegURL(string urlLink)
        {
            RegistryKey regKey = Registry.ClassesRoot.CreateSubKey(urlLink);
            RegistryKey cmdKey = regKey.CreateSubKey("shell");
            cmdKey = cmdKey.CreateSubKey("open");
            cmdKey = cmdKey.CreateSubKey("command");
            regKey.SetValue("URL Protocol", "");
            string filePath = Process.GetCurrentProcess().MainModule.FileName;
            cmdKey.SetValue("", "\"" + filePath + "\"" + " \"%1\"");
        }
        private static void UnRegURL(string urlLink)
        {
            Registry.ClassesRoot.DeleteSubKeyTree(urlLink);
        }
        private static bool IsAdmin()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
