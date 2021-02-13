using System;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;

namespace TrafficManagerLookup
{
	class Program
	{
		private static TimeSpan _minimumTtl = TimeSpan.FromSeconds(0);
		private static LookupClient _lookupClient;
		private static string _fqdn = "pztm-eus2.trafficmanager.net";
		private static int _numOfLookups = 100;

		static void Main(string[] args)
		{
			Init();

			Console.WriteLine($"Getting SB FQDN for Traffic Manager alias: {_fqdn}.");
			Console.WriteLine($"Number of lookups to do: {_numOfLookups}");
			Console.WriteLine("==================================================");

			Go().Wait();


			Console.WriteLine("Done");
			Console.ReadKey();
		}

		private static void Init()
		{
			LookupClientOptions options = new LookupClientOptions()
			{
				MinimumCacheTimeout = _minimumTtl
			};

			_lookupClient = new LookupClient(options);
		}

		private static async Task Go()
		{
			for (int i = 1; i <= _numOfLookups; i++)
			{
				var dnsQueryResponse = await _lookupClient.QueryAsync(_fqdn, QueryType.CNAME);
				string cname = dnsQueryResponse?.Answers?.CnameRecords()?.FirstOrDefault()?.CanonicalName?.Value;

				Console.WriteLine($"Got SB NS FQDN: {cname}");

				System.Threading.Thread.Sleep(200);
			}
		}
	}
}
