using System.Text.Json;
using AForge.Math.Metrics;

namespace mVectorSear
{
    internal class Program
    {
        public static Dictionary<string, double[]> SentenceList = new();

        static void Main(string[] args)
        {
            var api = new OpenAI_API.OpenAIAPI("")
            {
                ApiUrlFormat = "https://openai.api2d.net/{0}/{1}",
            };

            SentenceList = JsonSerializer.Deserialize<Dictionary<string, double[]>>(File.ReadAllText("test.json")) ?? new Dictionary<string, double[]>();

            Console.WriteLine("请输入：");

            var inputFloats = api.Embeddings.GetEmbeddingsAsync(Console.ReadLine()).Result;
            var similarities = new Dictionary<string, double>();

            foreach (var item in SentenceList)
            {
                var inputFloatsDoubles = Array.ConvertAll(inputFloats, x => (double)x);
                similarities.Add(item.Key, new CosineSimilarity().GetSimilarityScore(inputFloatsDoubles, item.Value));
            }

            similarities = similarities.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in similarities.TakeLast(10))
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
        }
    }
}