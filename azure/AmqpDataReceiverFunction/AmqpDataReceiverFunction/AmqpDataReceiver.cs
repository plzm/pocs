using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace AmqpDataReceiverFunction
{
    public static class AmqpDataReceiver
    {
        [FunctionName("AmqpDataReceiver")]
        public static void Run([ServiceBusTrigger("q1", Connection = "ServiceBusConnectionString")]Message message, ILogger log)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            string messageBody = string.Empty;
            string source = string.Empty;

            if (message.Body != null)
			{
                // Get message.Body as byte array
                byte[] messageBodyBytes = message.Body;

                // Decode message body byte array as UTF8 string
                messageBody = Encoding.UTF8.GetString(messageBodyBytes);

                source = "Body";
            }
            else
			{
                // If we have message body null problem per https://github.com/Azure/azure-sdk-for-net/issues/6912, access the Message SystemProperties BodyObject
                // This is an internal property inside a sealed child class of Message: https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/servicebus/Microsoft.Azure.ServiceBus/src/Message.cs#L512
                // To access its value, we use Reflection to retrieve the property value
                // NOTE that we are accessing an internal property. Future releases of Microsoft.Azure.ServiceBus may break this - there is NO GUARANTEE that this will keep working!
                try
				{
                    const string propName = "BodyObject";

                    object internalBodyObject =
                        message?
                        .SystemProperties?
                        .GetType()
                        .GetProperty(propName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic)?
                        .GetValue(message.SystemProperties, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, null, null)
                    ;

                    messageBody = internalBodyObject.ToString();

                    source = "Internal BodyObject";
                }
                catch (Exception ex)
				{
                    // TODO log exception

                    messageBody = string.Empty;

                    source = "(ERROR)";
                }
            }

            log.LogInformation($"Message Body: {messageBody} | Length: {(messageBody ?? string.Empty).Length} | Source: {source}");
        }
    }
}
