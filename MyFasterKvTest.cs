using FASTER.core;

namespace MyFasterTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var fasterKvSetting = new FasterKVSettings<string, string>("./fasterkv"); 
            using var fasterKv = new FasterKV<string, string>(fasterKvSetting);
            var session = fasterKv.For(new SimpleFunctions<string, string>())
                .NewSession<SimpleFunctions<string, string>>();

            session.Upsert("Hello", "World");
            for (int i = 0; i < 2048; i++) session.Upsert(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            session.Delete("Hello");
            session.Refresh();
            Console.WriteLine(session.Read("Hello").status.Found);
        }
    }
}