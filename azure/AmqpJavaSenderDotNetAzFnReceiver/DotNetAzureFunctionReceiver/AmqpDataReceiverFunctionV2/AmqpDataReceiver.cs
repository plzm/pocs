using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.ServiceBus;
using Azure.Core.Amqp;
using Azure.Messaging.ServiceBus;

namespace AmqpDataReceiverFunctionV2
{
    public static class AmqpDataReceiver
    {
        [FunctionName("AmqpDataReceiver")]
        public static void Run([ServiceBusTrigger("q1", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message, ILogger log)
        {
            string messageContent;

            // This works for byte[] messages
   //         using (var stream = message.Body.ToStream())
			//{
   //             stream.Seek(0, SeekOrigin.Begin);

   //             using (var reader = new StreamReader(stream))
			//	{
   //                 messageContent = reader.ReadToEnd();
   //             }
			//}

            // Text messages
            message.GetRawAmqpMessage().Body.TryGetValue(out var data);

            messageContent = data.ToString();

            log.LogInformation(messageContent);

            // ServiceBusReceivedMessage
            //if (message == null)
            //    throw new ArgumentNullException(nameof(message));

            ////bool ok = message.GetRawAmqpMessage().Body.TryGetData(out IEnumerable<ReadOnlyMemory<byte>> data);

            //AmqpAnnotatedMessage aam = message.GetRawAmqpMessage();
            //AmqpMessageBody amb = aam.Body;

            //bool ok = amb.TryGetValue(out object? ambValue);
        }
    }
}
