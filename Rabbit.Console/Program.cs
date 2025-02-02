using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json;
using Rabbit.Console.Services;


namespace Rabbit.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var hostName = ConfigurationManager.AppSettings["RabbitMQ:HostName"];

            var rabbitMQService= new RabbitMQService(hostName);


            var worker1 = new Thread(() => RunWorker1(rabbitMQService));
            var worker2 = new Thread(() => RunWorker2(rabbitMQService));

            worker1.Start();
            worker2.Start();

            Console.WriteLine("Workers iniciados. Pressione Enter para sair.");
            Console.ReadLine();
        }

        private static void RunWorker1(RabbitMQService rabbitMQService)
        {
            //Console.WriteLine("Worker 1 aguardando mensagens...");

            rabbitMQService.ConsumeMessages(message =>
            {
                var sumMessage = JsonConvert.DeserializeObject<SumMessage>(message);
                var result = sumMessage.Value1 + sumMessage.Value2;
                Console.WriteLine($"Worker 1 processou: {sumMessage.Value1} + {sumMessage.Value2} = {result}");
            }, "fila_somar");
        }

        private static void RunWorker2(RabbitMQService rabbitMQService)
        {
            //Console.WriteLine("Worker 2 aguardando mensagens...");

            rabbitMQService.ConsumeMessages(message =>
            {
                var sumMessage = JsonConvert.DeserializeObject<SumMessage>(message);
                var result = sumMessage.Value1 * sumMessage.Value2;
                Console.WriteLine($"Worker 2 processou: {sumMessage.Value1} * {sumMessage.Value2} = {result}");
            }, "fila_multiplicar");
        }
    }
}

public class SumMessage
{
    public double Value1 { get; set; }
    public double Value2 { get; set; }
}