using System;
using LukeSkywalker.IPNetwork;

namespace ipn
{
    class Program
    {
        static void Main(string[] args)
        {
            IPNetwork ipn = IPNetwork.Parse("192.168.168.100/24");
            Console.WriteLine(ipn);
            IPNetwork ipn2 = IPNetwork.Parse("192.168.168.100");
            Console.WriteLine(ipn2);
            Console.ReadKey();
        }
    }
}
