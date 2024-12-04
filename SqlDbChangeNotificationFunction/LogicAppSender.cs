using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Net.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace SqlDbChangeNotificationFunction
{
    public static class LogicAppSender
    {
        private static readonly HttpClient sharedClient = new HttpClient();

        public static async Task SendToLogicAppAsync(ToDoItem toDoItem, SqlChangeOperation operation, ILogger logger)
        {
            try
            {
                var logicAppUrl = Environment.GetEnvironmentVariable("LOGIC_APP_URL");
                if (string.IsNullOrEmpty(logicAppUrl))
                {
                    logger.LogError("LOGIC_APP_URL environment variable is not set.");
                    return;
                }

                var payload = JsonConvert.SerializeObject(new
                {
                    Operation = operation.ToString(),
                    ToDoItem = toDoItem
                });
                logger.LogInformation($"{payload}");

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await sharedClient.PostAsync(logicAppUrl, content);
                // response.EnsureSuccessStatusCode();

                logger.LogInformation("Successfully sent data to Logic App.");
            }
            catch(Exception ex)
            {
                logger.LogError($"Error sending data to Logic App: {ex.Message}");
                logger.LogError($"Exception details: {ex}");
            }
        }
    }
}