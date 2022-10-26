using CloudflareWorkersKv.Client;

var kvClient = new CloudflareWorkersKvClient<string>("test@example.com", "Global API Key", "Worker Account ID", "Worker KV Namespace ID");
kvClient.KeyValues.Write("hello", "world").GetAwaiter().GetResult();
Console.WriteLine(kvClient.KeyValues.Read("hello").Result);
Console.WriteLine(kvClient.KeyValues.List().Result.Keys.ToList().ToString());
