using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Infrastructure.Repositories
{
    public class RabbitMQRepository : IRabbitMQRepository, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQRepository()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "admin",
                Password = "admin123"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task PublishAsync<T>(T message, string queueName)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);

            await Task.CompletedTask;
        }

        public async Task StartConsumingAsync<T>(string queueName, Func<T, Task> handler)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                    
                    if (message != null)
                    {
                        await handler(message);
                    }

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
