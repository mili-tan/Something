using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace LemonLicenses
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(LemonLicensesActivate("38b1460a-5104-4067-a91d-77b872934d51",
                "JustTest", 1).msg);
        }

        public static (bool ok, string msg, int days, string instanceId) LemonLicensesActivate(string licenseKey,
            string instanceName, int productId)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"),
                "https://api.lemonsqueezy.com/v1/licenses/activate");

            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Content = new StringContent(string.Join("&", new List<string>
            {
                "license_key=" + licenseKey,
                "instance_name=" + instanceName
            }));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = httpClient.SendAsync(request).Result;

            var jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if (jObject.TryGetValue("activated", out var activatedToken))
                if (activatedToken.ToObject<bool>())
                {
                    if (jObject["meta"]!["product_id"]!.ToObject<int>() != productId)
                        return (false, "the product is not sold by us", 0, string.Empty);

                    var expiresDays = (jObject["license_key"]!["expires_at"]!.ToObject<DateTime>() - DateTime.Today)
                        .Days + 1;
                    var instanceId = jObject["instance"]!["id"]!.ToString();
                    return (true, "ok", expiresDays, instanceId);
                }
                else if (jObject.TryGetValue("error", out var errorToken))
                    return (false, errorToken.ToString(), 0, string.Empty);

            return (false, "license activation failed", 0, string.Empty);
        }
    }
}
