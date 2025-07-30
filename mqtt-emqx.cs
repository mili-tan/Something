using System.Reactive.Linq;
using MQTTnet;
using System.Reactive.Subjects;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MQTTTest
{
    internal class Program
    {
        private static IMqttClient mqttClient;
        private static Subject<(string topic, string payload)> messageSubject = new();

        static async Task Main(string[] args)
        {
            await File.WriteAllBytesAsync("emqxsl-ca.crt",
                await new HttpClient().GetByteArrayAsync("https://assets.emqx.com/data/emqxsl-ca.crt"));
          
            var factory = new MqttClientFactory();
            mqttClient = factory.CreateMqttClient();
            mqttClient.ConnectedAsync += async e =>
            {
                await mqttClient.SubscribeAsync("sensor/temperature");
            };

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine($"REV: {e.ApplicationMessage.ConvertPayloadToString()}");
                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("ala.asia-southeast1.emqxsl.com", 8883)
                .WithCredentials("user", "pass")
                .WithClientId(Guid.NewGuid().ToString())
                .WithCleanSession()
                .WithTlsOptions(x =>
                    x.UseTls().WithSslProtocols(SslProtocols.Tls12).WithCertificateValidationHandler(x => true)
                        .WithClientCertificates([new X509Certificate2("emqxsl-ca.crt")]).Build())
                .Build();

            await mqttClient.ConnectAsync(options);

            messageSubject
                .GroupBy(m => m.topic)
                .SelectMany(group =>
                    group.DistinctUntilChanged(m => m.payload)
                        .Throttle(TimeSpan.FromSeconds(15))
                        .Select(message => (group, message))
                ).Subscribe(async tuple =>
                {
                    var (group, message) = tuple;
                    if (!mqttClient.IsConnected) return;
                    await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic(message.topic)
                        .WithPayload(message.payload)
                        .Build());
                    Console.WriteLine($"PUB: {message.topic} - {message.payload}");
                });


            for (int i = 0; i < 10; i++)
            {
                messageSubject.OnNext(("sensor/temperature", "25.5"));
            }

            Console.ReadKey();
        }
    }
}
