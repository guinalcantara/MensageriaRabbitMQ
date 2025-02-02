using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Console.Services
{
    public class RabbitMQService
    {
        private readonly string _hostName;

        public RabbitMQService(string hostName)
        {
            _hostName = hostName;
        }

        public async Task ConsumeMessages(Action<string> onMessageReceived, string queueName)
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
                                            consumer: consumer);
        }
    }
}
