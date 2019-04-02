using System;
using System.Runtime.Caching;
using System.Threading;

namespace mMemoryCache
{
    static class Program
    {
        static void Main()
        {
            var cacheItem = new CacheItem("test", "just for test");
            var cachePolicy = new CacheItemPolicy(){SlidingExpiration = new TimeSpan(0,0,0,10)};
            MemoryCache.Default.Add(cacheItem, cachePolicy);

            Console.WriteLine(MemoryCache.Default.Contains("test").ToString());
            if (MemoryCache.Default.Contains("test"))
                Console.WriteLine(MemoryCache.Default["test"]);
            
            Thread.Sleep(11000);
            Console.WriteLine("Thread.Sleep(11000)");
            Console.WriteLine(MemoryCache.Default.Contains("test").ToString());

            Console.ReadKey();
        }
    }
}
