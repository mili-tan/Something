using Certify.ACME.Anvil;
using Certify.ACME.Anvil.Acme;
using Certify.ACME.Anvil.Acme.Resource;
using CloudFlare.Client;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;

namespace AutoLetsEncryptDnsChallenge
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string domain = "*.dns-01.example.com"; // 替换为您的域名
            const string email = "YOUR-EMAIL"; // 替换为您的电子邮件地址
            const string apiToken = "YOUR-CLOUDFLARE-API-TOKEN"; // 替换为您的 Cloudflare API 令牌
            const string zoneId = "YOUR-CLOUDFLARE-ZONE-ID"; // 替换为您的 Cloudflare 区域 ID

            try
            {
                Console.WriteLine($"开始为域名 {domain} 申请通配符证书...");

                var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);

                Console.WriteLine("创建ACME账户...");
                var account = await acme.NewAccount(email, true);

                Console.WriteLine("创建证书订单...");
                var order = await acme.NewOrder([domain]);

                var authz = (await order.Authorizations()).First();
                var dnsChallenge = await authz.Dns();

                var dnsTxt = acme.AccountKey.DnsTxt(dnsChallenge.Token);
                var recordName = $"_acme-challenge.{domain.Replace("*.", "")}";

                Console.WriteLine("\n=== DNS记录信息 ===");
                Console.WriteLine($"记录名称: {recordName}");
                Console.WriteLine($"记录值: {dnsTxt}");
                Console.WriteLine("===================\n");

                // 使用 Cloudflare API 自动添加 DNS 记录
                Console.WriteLine("正在通过 Cloudflare API 添加 DNS 记录...");

                var cloudFlareClient = new CloudFlareClient(apiToken);
                var dnsRecord = new NewDnsRecord
                {
                    Name = recordName,
                    Content = dnsTxt,
                    Type = DnsRecordType.Txt,
                    Ttl = 60,
                    Proxied = false
                };

                var result = await cloudFlareClient.Zones.DnsRecords.AddAsync(zoneId, dnsRecord);

                if (!result.Success)
                {
                    Console.WriteLine($"添加 DNS 记录失败: {string.Join(", ", result.Errors.Select(e => e.Message))}");
                    return;
                }

                Console.WriteLine($"DNS 记录添加成功！记录 ID: {result.Result.Id}");

                Console.WriteLine("等待 DNS 记录传播...");
                await Task.Delay(5000); // 等待5秒让 DNS 记录传播

                try
                {
                    Console.WriteLine("开始验证DNS记录...");
                    await dnsChallenge.Validate();

                    Console.WriteLine("等待验证完成...");
                    var challengeStatus = await WaitForValidation(dnsChallenge);

                    string pemCert, pemKey;

                    if (challengeStatus == ChallengeStatus.Valid)
                    {
                        Console.WriteLine("DNS验证成功！");
                        var certKey = KeyFactory.NewKey(KeyAlgorithm.ES256);

                        var csr = new CsrInfo
                        {
                            CommonName = domain,
                        };
                        try
                        {
                            Console.WriteLine("生成证书...");

                            var orderFinalize = await order.Finalize(csr, certKey);
                            for (int i = 0; i < 30; i++)
                            {
                                if (orderFinalize.Status == OrderStatus.Ready) break;
                                Thread.Sleep(1000);
                                var odrStatus = order.Resource().Result.Status;
                                Console.WriteLine(odrStatus);
                                if (odrStatus is OrderStatus.Ready or OrderStatus.Valid) break;
                            }

                            var cert = await order.Download();

                            pemCert = cert.ToPem();
                            pemKey = certKey.ToPem();

                            Console.WriteLine($"域名 {domain} 证书申请成功");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Thread.Sleep(3000);

                            Console.WriteLine("下载证书...");
                            var cert = await order.Download();

                            pemCert = cert.ToPem();
                            pemKey = certKey.ToPem();

                            Console.WriteLine($"域名 {domain} 证书申请成功");
                        }

                        Console.WriteLine("\n=== 证书申请成功 ===");
                        Console.WriteLine("证书内容:");
                        Console.WriteLine(pemCert);
                        Console.WriteLine("\n私钥内容:");
                        Console.WriteLine(pemKey);

                        // 保存证书文件
                        await File.WriteAllTextAsync("certificate.pem", pemCert);
                        await File.WriteAllTextAsync("privatekey.pem", pemKey);
                        Console.WriteLine("\n证书已保存到 certificate.pem 和 privatekey.pem");
                    }
                    else
                    {
                        Console.WriteLine($"DNS验证失败，状态: {challengeStatus}");
                    }
                }
                finally
                {
                    Console.WriteLine("清理 DNS 记录...");
                    var deleteResult = await cloudFlareClient.Zones.DnsRecords.DeleteAsync(zoneId, result.Result.Id);
                    Console.WriteLine(deleteResult.Success
                        ? "DNS 记录已成功删除"
                        : $"删除 DNS 记录失败: {string.Join(", ", deleteResult.Errors.Select(e => e.Message))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }
        }

        private static async Task<ChallengeStatus?> WaitForValidation(IChallengeContext challenge, int maxRetries = 30)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                var status = await challenge.Resource();

                if (status.Status == ChallengeStatus.Valid)
                    return status.Status;
                if (status.Status == ChallengeStatus.Invalid)
                    Console.WriteLine($"错误: {status.Error?.Detail ?? "未知错误"}");

                Console.WriteLine($"验证状态: {status.Status}，等待5秒后重试... ({i + 1}/{maxRetries})");
                await Task.Delay(5000);
            }

            return ChallengeStatus.Pending;
        }
    }
}
