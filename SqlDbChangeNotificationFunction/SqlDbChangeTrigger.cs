using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Azure.Communication.Email;

namespace SqlDbChangeNotificationSolution
{
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public int? order { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool? completed { get; set; }
    }

    public static class SqlDbChangeTrigger
    {
        [Function("SqlDbChangeTrigger")]
        public static void Run(
            [SqlTrigger("[dbo].[ToDo]", "SqlConnectionString")]
            IReadOnlyList<SqlChange<ToDoItem>> changes,
            FunctionContext context)
        {
            var logger = context.GetLogger("SqlDbChangeTrigger");
            logger.LogInformation("Function triggered.");

            if (changes == null || changes.Count == 0)
            {
                logger.LogInformation("No changes detected.");
                return;
            }

            foreach (SqlChange<ToDoItem> change in changes)
            {
                try
                {
                    ToDoItem toDoItem = change.Item;
                    logger.LogInformation($"Change operation: {change.Operation}");
                    logger.LogInformation($"Id: {toDoItem.Id}, Title: {toDoItem.title}, Url: {toDoItem.url}, Completed: {toDoItem.completed}");

                    // You can add additional processing logic here (e.g., send an email, update another service, etc.)
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing ToDoItem with Id {change.Item.Id}: {ex.Message}");
                    logger.LogError($"Exception details: {ex.ToString()}");
                }
            }

            // var connectionString = "ACS_CONNECTION_STRING";
            // var emailClient = new EmailClient(connectionString);

            // var emailContent = new EmailContent("Test Subject");
            // emailContent.PlainText = "Test Body";
        }
    }
}