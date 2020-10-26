using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BreadDuo
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderid = "";
            var urlkey = "";
            var creator = "";
            var key = "";

            Console.WriteLine(TryGetBreaduoPay(key, urlkey, orderid, out var msg, out var jObject));
            Console.WriteLine(msg);
            Console.WriteLine(jObject);
        }

        public static bool TryGetBreaduoPay(string breaduoKey, string urlKey, string orderId, out string msg, out JObject jObject)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-token", breaduoKey);
                var orderResult = client
                    .GetAsync("https://x.mianbaoduo.com/api/order-detail?order_id=" + orderId)
                    .Result;
                jObject = JObject.Parse(orderResult.Content.ReadAsStringAsync().Result);
                msg = jObject["error_info"].ToString();
                if (!orderResult.IsSuccessStatusCode) return false;
                if (jObject["code"].ToObject<int>() != 200) return false;
                if (jObject["result"]["state"].ToString() != "success")
                {
                    msg = "You may not have paid, please pay and try again later.";
                    return false;
                }

                if (jObject["result"]["urlkey"].ToString() != urlKey)
                {
                    msg = "Product type wrong, this may not be the product we sell.";
                    return false;
                }

                return true;
            }
            catch (HttpRequestException e)
            {
                jObject = new JObject();
                msg = e.Message;
                return false;
            }
        }
    }
}
