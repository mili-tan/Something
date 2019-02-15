using System;
using System.Linq;
using Ipfs.Http;

namespace IPFSTest
{
    public static class Program
    {
        public static void Main()
        {
            var ipfs = new IpfsClient("http://127.0.0.1:5002");
            Console.WriteLine(ipfs.FileSystem.ReadAllTextAsync("Qmf412jQZiuVUtdgnB36FXFX7xg5V6KEbSJ4dpQuhkLyfD").Result);
            Console.WriteLine(ipfs.FileSystem.AddTextAsync("just test").Result.Id);
            Console.WriteLine(ipfs.Swarm.PeersAsync().Result.Count());
            Console.ReadKey();
        }
    }
}
