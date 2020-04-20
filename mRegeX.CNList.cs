using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ARSoft.Tools.Net;

namespace mRegeX.ChinaName
{
    class Program
    {
        private static List<DomainName> chinaList = File.ReadAllLines("china_whitelist.list")
            .ToList().ConvertAll(DomainName.Parse);
        static void Main(string[] args)
        {
            //File.WriteAllLines("china_whitelist.list", list.ConvertAll(name => name.ToString().TrimEnd('.')));

            while (true)
            {
                var name = DomainName.Parse(Console.ReadLine());
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //foreach (var itemName in list)
                //{
                //    if (!name.IsEqualOrSubDomainOf(itemName)) continue;
                //    Console.WriteLine(itemName);
                //    break;
                //}
                Console.WriteLine(IsChinaName(name));
                stopWatch.Stop();
                Console.WriteLine("done:" + stopWatch.ElapsedTicks);
            }
        }

        public static bool IsChinaName(DomainName name)
        {
            return chinaList.Any(name.IsEqualOrSubDomainOf);
        }
    }
}
