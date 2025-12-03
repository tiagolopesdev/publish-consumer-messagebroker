using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Publish.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishController : ControllerBase
    {
        private readonly ConnectionFactory connectionFactory;
        private readonly IModel _channel;
        private const string QUEUE_NAME = "update-stock";

        public PublishController()
        {
            connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var bodyMessage = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new { Titulo = "Titulo aqui" })
                    );

                _channel.QueueDeclare(
                    queue: QUEUE_NAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                _channel.BasicPublish(exchange: "", routingKey: QUEUE_NAME, body: bodyMessage);

                return new OkObjectResult("Mensagem publicada com sucesso");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
