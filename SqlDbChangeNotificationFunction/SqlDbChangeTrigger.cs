using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Azure.Communication.Email;

namespace SqlDbChangeNotificationFunction
{
    public static class SqlDbChangeTrigger
    {
        [Function("SqlDbChangeTrigger")]
        public static async Task Run(
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
                    logger.LogInformation($"Id: {toDoItem.Id}, Order: {toDoItem.order}, Title: {toDoItem.title}, Url: {toDoItem.url}, Completed: {toDoItem.completed}");

                    await AcsEmailSender.SendEmailNotification(toDoItem, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing ToDoItem with Id {change.Item.Id}: {ex.Message}");
                    logger.LogError($"Exception details: {ex.ToString()}");
                }
            }
        }
    }
}