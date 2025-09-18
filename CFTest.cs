using CloudFlare.Client;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;

namespace CloudFlareTests
{
    public class Tests
    {
        private const string ZoneId = "your-test-zone-id";
        private const string ApiToken = "your-test-token-key";
        private const string Subdomain = "test.example.com";

        // 测试获取记录
        public static async Task TestGetRecords()
        {
            try
            {
                var client = new CloudFlareClient(ApiToken);
                var dnsRecords = await client.Zones.DnsRecords.GetAsync(ZoneId);

                Console.WriteLine("获取记录测试成功！");
                Console.WriteLine($"找到 {dnsRecords.Result.Count} 条记录");

                foreach (var record in dnsRecords.Result)
                {
                    Console.WriteLine($"记录: {record.Name} -> {record.Content} ({record.Type})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取记录测试失败: {ex.Message}");
            }
        }

        // 测试删除并重新添加记录
        public static async Task TestDeleteAndRecreateRecord()
        {
            try
            {
                var client = new CloudFlareClient(ApiToken);

                // 首先获取现有记录
                var dnsRecords = await client.Zones.DnsRecords.GetAsync(ZoneId);
                var existingRecord = dnsRecords.Result
                    .FirstOrDefault(r => r.Name == Subdomain &&
                                       (r.Type == DnsRecordType.A || r.Type == DnsRecordType.Cname));

                string originalContent = "";
                DnsRecordType originalType = DnsRecordType.A;

                if (existingRecord != null)
                {
                    // 保存原始值用于恢复
                    originalContent = existingRecord.Content;
                    originalType = existingRecord.Type;

                    // 删除记录
                    await client.Zones.DnsRecords.DeleteAsync(ZoneId, existingRecord.Id);
                    Console.WriteLine("已删除原有记录");
                }

                // 创建新记录（使用测试值）
                var newRecord = new NewDnsRecord()
                {
                    Name = Subdomain,
                    Type = DnsRecordType.Cname,
                    Content = "example.com", // 测试用的IP地址
                    Ttl = 300,
                    Proxied = false
                };

                var result = await client.Zones.DnsRecords.AddAsync(ZoneId, newRecord);
                Console.WriteLine($"已创建新记录: {newRecord.Content}");
                Console.WriteLine("删除并重新添加记录测试成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除并重新添加记录测试失败: {ex.Message}");
            }
        }

        // 运行测试
        public static async Task Main(string[] args)
        {
            Console.WriteLine("开始测试获取记录功能...");
            await TestGetRecords();

            Console.WriteLine("\n开始测试删除并重新添加记录功能...");
            await TestDeleteAndRecreateRecord();

            Console.WriteLine("\n所有测试完成！");
        }
    }
}
