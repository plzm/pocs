using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using System.Text;

namespace AmqpDataReceiverFunction
{
    public static class AmqpDataReceiver
    {
        [FunctionName("AmqpDataReceiver")]
        public static void Run([ServiceBusTrigger("q1", Connection = "ServiceBusConnectionString")]Message message, ILogger log)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // Get message.Body as byte array
            byte[] messageBodyBytes = message.Body;

            // Decode message body byte array as UTF8 string
			string messageBody = Encoding.UTF8.GetString(messageBodyBytes);

            log.LogInformation("Message ContentType: " + (message.ContentType ?? "Null"));
            log.LogInformation("Message Body Length: " + (messageBody.Length.ToString()));
			log.LogInformation("Message Body: " + (messageBody ?? "Null"));
        }
    }
}
