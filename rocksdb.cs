using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RocksDbSharp;

namespace DBBench
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var options = new DbOptions().SetCreateIfMissing(true);
            options.SetDbWriteBufferSize(16000000);
            var db = RocksDb.Open(options, "lists.db");

            var lists = FromGithubGetDomainLists();
            foreach (var list in lists)
            {
                foreach (var item in list.Value)
                {
                    Console.WriteLine(list.Key + ":" + item);
                    db.Put(list.Key + ":" + item, item);
                }
                lists.Remove(list.Key);
                list.Value.Clear();
                GC.Collect();
            }
            lists.Clear();
            db.Checkpoint();
            GC.Collect();

            while (true)
            {
                var input = Console.ReadLine();
                var swatch = new Stopwatch();
                swatch.Start();
                var output = db.Get(input);
                swatch.Stop();
                Console.WriteLine(output+":"+swatch.Elapsed.TotalMilliseconds);
            }

        }

        public static Dictionary<string, List<string>> FromGithubGetDomainLists(
            string repo = "mili-tan/justdomains", string branch = "lists")
        {
            try
            {
                var lists = new Dictionary<string, List<string>>();
                var jObject = JObject.Parse(
                    new WebClient { Headers = new WebHeaderCollection { "User-Agent:DotNetWebClient" } }.DownloadString(
                        $"https://api.github.com/repos/{repo}/git/trees/{branch}?recursive=1"));

                Task.WaitAll(jObject["tree"]
                    .Select(jToken => jToken["path"].ToString())
                    .Where(listPath => listPath.StartsWith("lists/"))
                    .Select(listPath => Task.Run(() =>
                    {
                        try
                        {
                            lists.Add(listPath.Replace("lists/", string.Empty),
                                FromUrlGetStringsList($"https://cdn.jsdelivr.net/gh/{repo}@{branch}/{listPath}"));
                            Console.WriteLine(listPath + " OK!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    })).ToArray());
                return lists;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Dictionary<string, List<string>>();
            }
        }

        public static List<string> FromUrlGetStringsList(string url)
        {
            try
            {
                var list = new HttpClient().GetStringAsync(url).Result
                    .ToLower().Split("\n").Select(item =>
                        !item.StartsWith("#") || string.IsNullOrWhiteSpace(item)
                            ? item.Trim()
                            : string.Empty).ToList();
                list.RemoveAll(item => item == String.Empty);
                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<string>();
            }
        }
    }
}
