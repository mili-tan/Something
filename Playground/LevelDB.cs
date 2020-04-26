using System;
using LevelDB;

namespace mLevelDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DB(new Options {CreateIfMissing = true}, "my.db");
            Console.WriteLine(string.IsNullOrWhiteSpace(db.Get("try")));
            db.Put("aa:a", "dbdb");
            db.Put("test:tt","dbdb");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
                db.Put($"user:{i}:{Guid.NewGuid()}",Guid.NewGuid().ToString());
                db.Put("test:" + i, "dbdb");
            }

            using (var it = db.CreateIterator())
            {
                for (it.Seek("user:"); it.IsValid(); it.Next())
                {
                    Console.WriteLine("Value as string: {0}", it.KeyAsString());
                }
            }
            Console.ReadKey();
        }
    }
}
