using System;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json.Linq;

namespace JWToken
{
    class Program
    {
        static void Main(string[] args)
        {
            var secret = Guid.NewGuid();
            var token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret.ToString())
                .ExpirationTime(DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds())
                .IssuedAt(DateTime.Now)
                .AddClaim("token", secret)
                .Encode();
            Console.WriteLine(token);

            var json = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret.ToString())
                .MustVerifySignature()
                .Decode<JObject>(token);
            Console.WriteLine(json["token"].ToString());
        }
    }
}
