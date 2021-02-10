using System;
using Sharprompt;

namespace SharpromptTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var city = Prompt.Select("Select your city", new[] { "Seattle", "London", "Tokyo" });
            Console.WriteLine($"Hello, {city}!");
            var answer = Prompt.Confirm("Are you ready?");
            Console.WriteLine($"Your answer is {answer}");
        }
    }
}
