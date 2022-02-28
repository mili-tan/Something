using System;
using System.Text;
using uplink.NET.Models;
using uplink.NET.Services;

namespace UpLinkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var access = new Access("grant-here");
            var bucketService = new BucketService(access);
            var buckets = bucketService.ListBucketsAsync(new ListBucketsOptions()).Result;
            foreach (var item in buckets.Items)
            {
                Console.WriteLine(item.Name);
            }

            var bucket = bucketService.GetBucketAsync("test").Result;

            var objectService = new ObjectService(access);
            var bytesToUpload = Encoding.UTF8.GetBytes("Storj is awesome!");

            for (int i = 0; i < 5; i++)
            {
                objectService
                    .UploadObjectAsync(bucket, $"test-time/{Guid.NewGuid()}.txt", new UploadOptions { Expires = DateTime.UtcNow.AddDays(1) },
                        bytesToUpload, false).Result
                    .StartUploadAsync().GetAwaiter().GetResult();
            }
            var objectList = objectService.ListObjectsAsync(bucket, new ListObjectsOptions
            {
                Prefix = "test-time/", Recursive = true, System = true
            }).Result;

            foreach (var item in objectList.Items)
            {
                Console.WriteLine(item.Key + ":" + item.SystemMetadata.Created);
            }
        }
    }
}
