using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SqlDbChangeNotificationFunction
{
    public static class AcsEmailSender
    {
        public static async Task SendEmailNotification(ToDoItem toDoItem, ILogger logger)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("ACS_CONNECTION_STRING");

                var senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
                var recipientString = Environment.GetEnvironmentVariable("RECIPIENT_EMAILS");

                var emailClient = new EmailClient(connectionString);

                var emailContent = new EmailContent($"Change Notification for {toDoItem.title}");
                emailContent.PlainText = $"""
                There has been a change operation to the database table ToDo.

                Updated ToDoItem:
                Id: {toDoItem.Id}
                Order: {toDoItem.order}
                Title: {toDoItem.title}
                Url: {toDoItem.url}
                Completed: {toDoItem.completed}
                """;

                var recipientList = recipientString?.Split(",")
                    .Select(email => new EmailAddress(email.Trim()))
                    .ToList() ?? new List<EmailAddress>();
                
                var recipientObject = new EmailRecipients(recipientList);

                var emailMessage = new EmailMessage(senderEmail, recipientObject, emailContent);

                await emailClient.SendAsync(Azure.WaitUntil.Started, emailMessage);
                logger.LogInformation($"Email notification sent for ToDoItem with Id {toDoItem.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending email notification for ToDoItem with Id {toDoItem.Id}: {ex.Message}");
                logger.LogError($"Exception details: {ex}");
            }
        }
    }
}