using System;
using System.Text;
using RocksDbSharp;

namespace mRocksDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbOptions = new DbOptions().SetCreateIfMissing().SetCreateMissingColumnFamilies();
            var columnFamilies = new ColumnFamilies
            {
                {"default", new ColumnFamilyOptions().OptimizeForPointLookup(256)},
                {
                    "user", new ColumnFamilyOptions()
                        .SetMemtableHugePageSize(2 * 1024 * 1024)
                        .SetPrefixExtractor(SliceTransform.CreateFixedPrefix(5))
                        .SetBlockBasedTableFactory(
                            new BlockBasedTableOptions().SetFilterPolicy(BloomFilterPolicy.Create(10, false))
                            .SetWholeKeyFiltering(false))
                }
            };
            var db = RocksDb.Open(dbOptions, "my.db", columnFamilies);
            var testFamily = db.GetColumnFamily("user");
            
            db.Put("hello:hi","hi");
            for (var i = 0; i < 5; i++) db.Put($"user:{Guid.NewGuid()}", Guid.NewGuid().ToString(), testFamily);
            using (var it = db.NewIterator(testFamily))
            {
                it.Seek(Encoding.UTF8.GetBytes("user:"));
                while (it.Valid())
                {
                    Console.WriteLine(it.StringKey());
                    it.Next();
                }
            }

            Console.WriteLine("Done!" + db.Get("hello:hi"));
        }
    }
}
