using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Minio;
using Minio.DataModel;

namespace MinioS3
{
    class Program
    {
        static void Main(string[] args)
        {
            var minio = new MinioClient("s3.us-west-000.backblazeb2.com",
                "KeyID",
                "ApplicationKey"
            ).WithSSL();

            var buckets = minio.ListBucketsAsync().Result.Buckets;
            foreach (Bucket bucket in buckets) Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);

            var s = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            minio.PutObjectAsync(buckets.FirstOrDefault().Name, "/test/test.txt", s, s.Length,
                "text/plain").Wait();

            var items = minio.ListObjectsAsync(buckets.FirstOrDefault().Name, "/test/").ToList().ToTask().Result;
            foreach (var i in items) Console.WriteLine(i.Key);

            minio.GetObjectAsync(buckets.FirstOrDefault().Name, "/test/test.txt",
                stream => Console.WriteLine(new StreamReader(stream, Encoding.UTF8).ReadToEnd())).Wait();
        }
    }
}
