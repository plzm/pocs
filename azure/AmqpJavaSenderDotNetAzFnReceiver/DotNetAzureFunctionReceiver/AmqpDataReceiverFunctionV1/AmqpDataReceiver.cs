using System;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Extensions.Logging;

namespace AmqpDataReceiverFunction
{
	public static class AmqpDataReceiver
	{
		[FunctionName("AmqpDataReceiver")]
		public static void Run([ServiceBusTrigger("q1", Connection = "ServiceBusConnectionString")] Message message, ILogger log)
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
				// If we have message body null per https://github.com/Azure/azure-sdk-for-net/issues/6912, access the Message SystemProperties BodyObject
				// using the extension method GetBody<T>() - https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/servicebus/Microsoft.Azure.ServiceBus/src/Extensions/MessageInterOpExtensions.cs#L76
				// Pass null for the XmlSerializer and String for the generic type to bypass deserialization internally
				try
				{
					messageBody = message.GetBody<String>(null);
					source = "BodyObject";
				}
				catch (Exception ex)
				{
					log.LogCritical(ex.Message);

					messageBody = string.Empty;
					source = "(ERROR)";
				}
			}

			log.LogInformation($"Message Body: {messageBody} | Length: {(messageBody ?? string.Empty).Length} | Source: {source}");
		}
	}
}
