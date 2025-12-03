using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ConsumerConsole
{
    public class ConsumerListening : BackgroundService
    {
        private const string QUEUE_NAME = "update-stock";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(QUEUE_NAME, durable: true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                Console.WriteLine($"Nova mensagem recebida: {DateTime.Now}");
                
                // PROCESS TO MESSAGE

                var stringMessage = Encoding.UTF8.GetString(args.Body.ToArray());

                var test = JsonSerializer.Deserialize<dynamic>(stringMessage);

                channel.BasicAck(args.DeliveryTag, false);
            };
            /*
             consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var stringMessage = Encoding.UTF8.GetString(body);
                T foundedObject = JsonSerializer.Deserialize<T>(stringMessage);
            };
             */

            channel.BasicConsume(QUEUE_NAME, autoAck: false, consumer);

            Console.WriteLine("Consumidor iniciado e aguardando mensagens...");

            // THIS CODE KEPP ON SERVICE, IT NOT LISTENING MESSAGENS
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
