using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mOngodDbTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new MongoClient("mongodb+mask");
            var database = client.GetDatabase("mJump");
            var collection = database.GetCollection<Entity>("mJump");
            collection.Indexes.CreateOne(
                new CreateIndexModel<Entity>(Builders<Entity>.IndexKeys.Text(x => x.Name)));

            if (collection.Find(x => x.Name == "Jack").CountDocuments() == 0)
                collection.InsertOne(new Entity() {Name = "Jack", RdID = "test"});
            else
                Console.WriteLine("已存在");

            Console.WriteLine(collection.Find(x => x.Name == "Jack").FirstOrDefault().RdID);

            Console.WriteLine("Hello World!");
        }

        public class Entity
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public string RdID { get; set; }
        }
    }
}
