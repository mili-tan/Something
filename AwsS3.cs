using System;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsS3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var config = new AmazonS3Config
            {
                ServiceURL = "https://s3.us-west-000.backblazeb2.com/"
            };

            var s3Client = new AmazonS3Client(
                "",
                "",
                config
            );

            var buckets = s3Client.ListBucketsAsync().Result.Buckets;
            foreach (var b in buckets) Console.WriteLine(b.BucketName + " " + b.CreationDate);
            
            s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = buckets.First().BucketName,
                Key = $"/t/{Guid.NewGuid()}.txt",
                ContentType = "text/plain",
                ContentBody = "Hello World!"
            }).Wait();

            var objects = s3Client.ListObjectsAsync(buckets.FirstOrDefault().BucketName, "/t/").Result.S3Objects;
            foreach (var i in objects) Console.WriteLine(i.Key);

            var stream = s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = buckets.First().BucketName,
                Key = "/t/hello.txt"
            }).Result.ResponseStream;
            Console.WriteLine(new StreamReader(stream, Encoding.UTF8).ReadToEnd());
        }
    }
}
