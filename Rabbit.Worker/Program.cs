using Newtonsoft.Json;
using Rabbit.Worker.Services;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Worker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var hostName = ConfigurationManager.AppSettings["RabbitMQ:HostName"];

            var rabbitMQService = new RabbitMQService(hostName);

            var cts = new CancellationTokenSource();

            var worker1 = Task.Run(() => RunWorker1Async(rabbitMQService, cts.Token));
            var worker2 = Task.Run(() => RunWorker2Async(rabbitMQService, cts.Token));

            Console.WriteLine("Workers iniciados. Pressione Enter para sair.");
            Console.ReadLine();

            cts.Cancel();

            await Task.WhenAll(worker1, worker2);
        }

        private static async Task RunWorker1Async(RabbitMQService rabbitMQService, CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker Somar aguardando...");

            await rabbitMQService.ConsumeMessages(message =>
            {
                var sumMessage = JsonConvert.DeserializeObject<SumMessage>(message);
                var result = sumMessage.Value1 + sumMessage.Value2;
                Console.WriteLine($"Worker 1 processou: {sumMessage.Value1} + {sumMessage.Value2} = {result} Data: {DateTime.Now.Ticks}");
            }, "fila_somar", cancellationToken);
        }

        private static async Task RunWorker2Async(RabbitMQService rabbitMQService, CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker Multiplicar aguardando...");

            await rabbitMQService.ConsumeMessages(message =>
            {
                var sumMessage = JsonConvert.DeserializeObject<SumMessage>(message);
                var result = sumMessage.Value1 * sumMessage.Value2;
                Console.WriteLine($"Worker 2 processou: {sumMessage.Value1} * {sumMessage.Value2} = {result}");
            }, "fila_multiplicar", cancellationToken);
        }
    }
}
public class SumMessage
{
    public double Value1 { get; set; }
    public double Value2 { get; set; }
}
