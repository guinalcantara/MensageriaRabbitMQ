using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Worker.Services
{
    public class RabbitMQService
    {
        private readonly string _hostName;

        public RabbitMQService(string hostName)
        {
            _hostName = hostName;
        }

        public async Task ConsumeMessages(Action<string> onMessageReceived, string queueName, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                onMessageReceived(message);
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                            autoAck: true,
                                            consumer: consumer,
                                            cancellationToken: cancellationToken);
        }
    }
}
