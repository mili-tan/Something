using Microsoft.DeepDev;

namespace mTokenCount
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var IM_START = "<|im_start|>";
            var IM_END = "<|im_end|>";

            var specialTokens = new Dictionary<string, int>{
                { IM_START, 100264},
                { IM_END, 100265},
            };
            var tokenizer = TokenizerBuilder.CreateByModelName("gpt-4", specialTokens);

            var text = "<|im_start|>你好！如有需要，我隨時提供幫助。<|im_end|>";
            var encoded = tokenizer.Encode(text, new HashSet<string>(specialTokens.Keys));
            Console.WriteLine(encoded.Count);
            Console.WriteLine(string.Join(",", encoded));
            var decoded = tokenizer.Decode(encoded.ToArray());
            Console.WriteLine(decoded);
            Console.WriteLine("Hello, World!");
        }
    }
}