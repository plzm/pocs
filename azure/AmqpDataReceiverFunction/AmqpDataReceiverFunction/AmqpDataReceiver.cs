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
            byte[] bytes = message.Body;

            string body = Encoding.UTF8.GetString(bytes);
            log.LogInformation("Body from Bytes[] payload: " + (body ?? "Null"));

            
        }
    }
}
