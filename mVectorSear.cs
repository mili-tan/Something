using System.Text.Json;
using AForge.Math.Metrics;

namespace mVectorSear
{
    internal class Program
    {
        public static Dictionary<string, double[]> SentenceDictionary = new();

        static void Main(string[] args)
        {
            var api = new OpenAI_API.OpenAIAPI("")
            {
                ApiUrlFormat = "https://openai.api2d.net/{0}/{1}",
            };

            SentenceDictionary = JsonSerializer.Deserialize<Dictionary<string, double[]>>(File.ReadAllText("test.json")) ??
                                 new Dictionary<string, double[]>();

            Console.WriteLine("请输入：");

            var similarities = new Dictionary<string, double>();

            var inputDoubles = Array.ConvertAll(api.Embeddings.GetEmbeddingsAsync(Console.ReadLine()).Result, x => (double) x);
            foreach (var pair in SentenceDictionary) similarities.Add(pair.Key, new CosineSimilarity().GetSimilarityScore(inputDoubles, pair.Value));

            foreach (var item in similarities.OrderBy(x => x.Value).TakeLast(10).Reverse())
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
        }
    }
}
