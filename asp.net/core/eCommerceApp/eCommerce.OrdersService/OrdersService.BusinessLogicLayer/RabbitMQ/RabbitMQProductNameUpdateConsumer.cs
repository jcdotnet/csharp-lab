using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrdersService.BusinessLogicLayer.RabbitMQ
{
    internal class RabbitMQProductNameUpdateConsumer : IDisposable, IRabbitMQProductNameUpdateConsumer
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQProductNameUpdateConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQProductNameUpdateConsumer(IConfiguration configuration,
            ILogger<RabbitMQProductNameUpdateConsumer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        public async Task ConsumeAsync()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RABBITMQ_HOST"]!,
                Port = Convert.ToInt32(_configuration["RABBITMQ_PORT"]!),
                UserName = _configuration["RABBITMQ_USER"]!,
                Password = _configuration["RABBITMQ_PASS"]!,
            };

            _connection = await connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            string routingKey = "product.update.name"; // binding key
            string queueName = "orders.product.update.name.queue";

            string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;
            await _channel.ExchangeDeclareAsync(exchangeName, type: ExchangeType.Direct, durable: true);

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            await _channel.QueueBindAsync(queueName, exchangeName, routingKey);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            // the Received event will be fired as soon as the message is received in the message queue
            // i.e. a delivery arrives for the consumer
            consumer.ReceivedAsync += async (sender, args) =>
            {
                byte[] body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                var product = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);

                _logger.LogInformation($"Product name updated to {product?.UpdatedProductName}");
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                consumer: consumer,
                autoAck: true
            );
            // waiting for messages...
        }
    }
}
