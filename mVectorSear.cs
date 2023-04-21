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

            SentenceList = JsonSerializer.Deserialize<Dictionary<string, double[]>>(File.ReadAllText("test.json")) ??
                           new Dictionary<string, double[]>();

            Console.WriteLine("请输入：");

            var inputDoubles = Array.ConvertAll(api.Embeddings.GetEmbeddingsAsync(Console.ReadLine()).Result,
                x => (double) x);
            var similarities = SentenceList.ToDictionary(item => item.Key,
                item => new CosineSimilarity().GetSimilarityScore(inputDoubles, item.Value));

            similarities = similarities.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in similarities.TakeLast(10).Reverse())
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
        }
    }
}
