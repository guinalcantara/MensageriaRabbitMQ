using Newtonsoft.Json;
using Rabbit.Api.Services;
using Rabbit.Repositorio;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rabbit.Api.Controllers
{
    public class SyncController : ApiController
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly LogRepository _logRepository;

        public SyncController()
        {
            var hostName = ConfigurationManager.AppSettings["RabbitMQ:HostName"];

            _rabbitMQService = new RabbitMQService(hostName);
            _logRepository = new LogRepository();
        }

        [HttpPost]
        [Route("api/somar")]
        public async Task<IHttpActionResult> SomarValores([FromBody] SumMessage message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);

            //salvar json para consulta de processamento
            var log = new Log
            {
                Message = jsonMessage,
                Data = DateTime.Now
            };
            await _logRepository.SaveLogAsync(log);

            await _rabbitMQService.PublishMessageAsync(message, "fila_somar");

            return Ok($"Valores '{message.Value1}' e '{message.Value2}' enviados para o RabbitMQ.");
        }

        [HttpPost]
        [Route("api/multiplicar")]
        public async Task<IHttpActionResult> MultiplicarValores([FromBody] SumMessage message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);

            //salvar json para consulta de processamento
            var log = new Log
            {
                Message = jsonMessage,
                Data = DateTime.Now
            };
            await _logRepository.SaveLogAsync(log);

            await _rabbitMQService.PublishMessageAsync(message, "fila_multiplicar");

            return Ok($"Valores '{message.Value1}' e '{message.Value2}' enviados para o RabbitMQ.");
        }

        [HttpGet]
        [Route("api/logs")]
        public async Task<IHttpActionResult> ObterLogs()
        {
            var logs = await _logRepository.GetLogsAsync();

            return Ok(logs);
        }
    }
}


public class SumMessage
{
    public double Value1 { get; set; }
    public double Value2 { get; set; }
}