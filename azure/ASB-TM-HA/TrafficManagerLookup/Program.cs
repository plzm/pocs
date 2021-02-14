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
		private static int _sendCount = 100;

		// PROVIDE THESE VALUES
		private static string _aliasFqdn = "PROVIDE.trafficmanager.net";
		private static string _asbSharedAccessKeyName = "";
		private static string _asbSharedAccessKeyValue = "";
		private static string _queueName = "";

		static void Main(string[] args)
		{
			if (string.IsNullOrWhiteSpace(_asbSharedAccessKeyName) || string.IsNullOrWhiteSpace(_asbSharedAccessKeyValue) || string.IsNullOrWhiteSpace(_queueName))
			{
				Console.WriteLine("Hey, how about some values for ASB key and entity name! Fix and try again.");
			}
			else
			{
				Go().Wait();
			}

			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}

		private static async Task Go()
		{
			Lookup lookup = new Lookup(TimeSpan.FromSeconds(0));
			Sender sender = new Sender();

			for (int i = 1; i <= _sendCount; i++)
			{
				string asbFqdn = lookup.QueryAsync(_aliasFqdn, QueryType.CNAME).Result;

				Console.WriteLine($"Sending message to {asbFqdn}");

				await sender.SendQueueMessageAsync(asbFqdn, _asbSharedAccessKeyName, _asbSharedAccessKeyValue, _queueName, $"Message {i}");

				System.Threading.Thread.Sleep(100);
			}
		}
	}
}
