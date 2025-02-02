using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Rabbit.Repositorio
{
    public class LogRepository
    {
        private readonly string _connectionString;

        public LogRepository()
        {
            //_connectionString = ConfigurationManager.AppSettings["MySQL:ConnectionString"];
            _connectionString = "Server=localhost;Port=3306;Database=LogDb;Uid=root;Pwd=123456;";
        }

        public async Task SaveLogAsync(Log log)
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO Logs (Message, Data) VALUES (@Message, @Data)", connection);
            command.Parameters.AddWithValue("@Message", log.Message);
            command.Parameters.AddWithValue("@Data", log.Data);

            await command.ExecuteNonQueryAsync();
        }

       public async Task<List<Log>> GetLogsAsync()
        {
            var logs = new List<Log>();

            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT Id, Message, Data FROM Logs", connection);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logs.Add(new Log
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")), // Verificação para valores NULL
                    Message = reader.IsDBNull(reader.GetOrdinal("Message")) ? null : reader.GetString(reader.GetOrdinal("Message")),
                    Data = reader.IsDBNull(reader.GetOrdinal("Data")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("Data"))
                });
            }

            return logs;
        }
    }
}

public class Log
{
    public int? Id { get; set; }
    public string Message { get; set; }
    public DateTime Data { get; set; }
}
