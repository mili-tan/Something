using System;
using SiaSkynet;

namespace SiaSkyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var sky = new SiaSkynetClient();
            //var upload = sky.UploadFileAsync("next.tar", File.OpenRead("next.tar"));
            //var skyResult = upload.GetAwaiter().GetResult();
            //Console.WriteLine(skyResult.Skylink);
            //Console.WriteLine(skyResult.Merkleroot);
            //Console.WriteLine(skyResult.Bitfield);

            //var download = sky.DownloadFileAsStringAsync("AAC0uO43g64ULpyrW0zO3bjEknSFbAhm8c-RFP21EQlmSQ");
            //Console.WriteLine(download.Result.file);
            
            var key = SiaSkynetClient.GenerateKeys("milkey");
            var dbGet = sky.SkyDbGetAsString(key.publicKey, "blazor-sample");
            Console.WriteLine(dbGet.Result);

            var dbSet = sky.SkyDbSet(key.privateKey, key.publicKey, "blazor-sample", "hello");
            Console.WriteLine(dbSet.Result);
        }
    }
}
