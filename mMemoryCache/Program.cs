using System;
using System.Linq;
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
            MemoryCache.Default.Add(new CacheItem("bala","balabala"), new CacheItemPolicy(){SlidingExpiration = TimeSpan.Zero});

            Console.WriteLine(MemoryCache.Default.Contains("test").ToString());
            if (MemoryCache.Default.Contains("test"))
                Console.WriteLine(MemoryCache.Default["test"]);
            Console.WriteLine(MemoryCache.Default.Count());
            
            Thread.Sleep(11000);
            Console.WriteLine("Thread.Sleep(11000)");
            Console.WriteLine(MemoryCache.Default.Contains("test").ToString());
            Console.WriteLine(MemoryCache.Default.Count());

            MemoryCache.Default.Dispose();
            Console.WriteLine("MemoryCache.Default.Dispose()");
            Console.WriteLine(MemoryCache.Default.Contains("test").ToString());
            Console.WriteLine(MemoryCache.Default.Count());

            Console.ReadKey();
        }
    }
}
