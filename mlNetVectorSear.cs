using AForge.Math.Metrics;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

class mlNetVectorSear
{
    static void Main(string[] args)
    {

        var data = new[]
        {
            new {Text = "This is a sentence."},
            new {Text = "This is another sentence."}
        }.ToList();

        //foreach (var item in File.ReadAllLines("test.txt"))
        //    if (!string.IsNullOrWhiteSpace(item)) data.Add(new {Text = item});

        var mlContext = new MLContext();
        var dataView = mlContext.Data.LoadFromEnumerable(data);
        var dictionary = GetFloatsMap(mlContext, dataView);

        foreach (var entry in dictionary) 
            Console.WriteLine($"{entry.Key}: {string.Join(" ", entry.Value)}");

        Console.WriteLine("请输入：");

        var inputDoubles = GetFloatsMap(mlContext, mlContext.Data.LoadFromEnumerable(new[]
        {
            new {Text = Console.ReadLine()}
        })).FirstOrDefault().Value;

        var similarities = new Dictionary<string, double>();
        foreach (var pair in dictionary)
            similarities.Add(pair.Key, new CosineSimilarity().GetSimilarityScore(inputDoubles, pair.Value));

        foreach (var item in similarities.OrderBy(x => x.Value).TakeLast(10).Reverse())
            Console.WriteLine(item.Key + ":" + item.Value);
    }

    public static Dictionary<string, double[]> GetFloatsMap(MLContext mlContext,IDataView dataView)
    {
        var embeddingEstimator = mlContext.Transforms.Text.NormalizeText(
                "NormalizeText", "Text")
            .Append(mlContext.Transforms.Text.TokenizeIntoWords(
                "Tokens", "NormalizeText"))
            .Append(mlContext.Transforms.Text.ApplyWordEmbedding(
                "Features",
                "Tokens",
                WordEmbeddingEstimator.PretrainedModelKind.SentimentSpecificWordEmbedding
            ));

        var embeddingTransformer = embeddingEstimator.Fit(dataView);
        var transformedDataView = embeddingTransformer.Transform(dataView);

        var floats = transformedDataView.GetColumn<float[]>("Features");
        var texts = transformedDataView.GetColumn<string>("Text");

        var doubles = floats.Select(i => Array.ConvertAll(i.ToArray(), x => (double)x));

        return texts.Zip(doubles, (k, v) => new {Key = k, Value = v})
            .ToDictionary(x => x.Key, x => x.Value);
    }
}