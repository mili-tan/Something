using System;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;

namespace ETCD_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var etcdClient = new EtcdClient("http://localhost:2379");
            etcdClient.DeleteRange("/foo");
            //Console.WriteLine(etcdClient.GetVal("/foo"));
            //Console.WriteLine(etcdClient.Get("foo").Kvs.FirstOrDefault().Value.ToStringUtf8());
            etcdClient.Put($"/foo/{Guid.NewGuid()}", Guid.NewGuid().ToString());
            etcdClient.Put(new PutRequest
            {
                Key = ByteString.CopyFromUtf8("/time"),
                Value = ByteString.CopyFromUtf8(Guid.NewGuid().ToString()),
                Lease = etcdClient.LeaseGrant(new LeaseGrantRequest
                {
                    TTL = 20
                }).ID
            });
            var response = etcdClient.GetRange("/");
            foreach (var keyValue in response.Kvs)
            {
                Console.WriteLine($"{keyValue.Key.ToStringUtf8()}:{keyValue.Value.ToStringUtf8()}");
            }
            Console.ReadKey();
        }
    }
}
