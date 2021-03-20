using System;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using TrafficMgrLookup;
using MessageProducer;
using System.Diagnostics;

namespace AsbTmHa
{
	class Program
	{
		private static int _sendCount = 10;

		// PROVIDE THESE VALUES
		private static string _aliasFqdn = "PROVIDE.trafficmanager.net";
		private static string _asbSharedAccessKeyName = "";
		private static string _asbSharedAccessKeyValue = "";
		private static string _queueName = "q1";
		private static string _asbConnString = "Endpoint=sb://PROVIDE.servicebus.windows.net/;SharedAccessKeyName=SendListen;SharedAccessKey=PROVIDE";

		static void Main(string[] args)
		{
			//if (string.IsNullOrWhiteSpace(_asbSharedAccessKeyName) || string.IsNullOrWhiteSpace(_asbSharedAccessKeyValue) || string.IsNullOrWhiteSpace(_queueName))
			//{
			//	Console.WriteLine("Hey, how about some values for ASB key and entity name! Fix and try again.");
			//}
			//else
			//{
				Go().Wait();
			//}

			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}

		//private static async Task Go()
		//{
		//	Lookup lookup = new Lookup(TimeSpan.FromSeconds(0));
		//	Sender sender = new Sender();

		//	for (int i = 1; i <= _sendCount; i++)
		//	{
		//		string asbFqdn = lookup.QueryAsync(_aliasFqdn, QueryType.CNAME).Result;

		//		Console.WriteLine($"Sending message to {asbFqdn}");

		//		await Send(sender, asbFqdn, $"Message {i}");

		//		System.Threading.Thread.Sleep(100);
		//	}
		//}

		private static async Task Go()
		{
			Sender sender = new Sender();

			for (int i = 1; i <= _sendCount; i++)
			{
				await Send(sender, _asbConnString, $"Message {i}");

				System.Threading.Thread.Sleep(100);
			}
		}

		private static async Task Send(Sender sender, string _asbConnString, string message)
		{
			await sender.SendQueueMessageAsync(_asbConnString, _queueName, message);
		}
	}
}
