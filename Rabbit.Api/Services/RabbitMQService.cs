using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RabbitMQ.Client.Events;
using System.Threading;


namespace Rabbit.Api.Services
{
    public class RabbitMQService
    {
        private readonly string _hostName;

        public RabbitMQService(string hostName)
        {
            _hostName = hostName;
        }

        public async Task PublishMessageAsync<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.BasicPublishAsync(exchange: "",
                                            routingKey: queueName,
                                            mandatory: false,
                                            body: body);

        }
    }
}