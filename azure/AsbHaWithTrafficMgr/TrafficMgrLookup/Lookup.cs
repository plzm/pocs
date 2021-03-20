using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace TrafficMgrLookup
{
	public class Lookup
	{
		private static LookupClientOptions _lookupClientOptions = null;
		private static LookupClient _lookupClient = null;

		public TimeSpan? MinimumTtl { get; private set; } = null;
		public TimeSpan? MaximumTtl { get; private set; } = null;

		public LookupClientOptions LookupClientOptions
		{
			get
			{
				if (_lookupClientOptions == null)
				{
					_lookupClientOptions = new LookupClientOptions()
					{
						MinimumCacheTimeout = this.MinimumTtl,
						MaximumCacheTimeout = this.MaximumTtl,
					};
				}

				return _lookupClientOptions;
			}
		}

		public LookupClient LookupClient
		{
			get
			{
				if (_lookupClient == null)
					_lookupClient = new LookupClient(this.LookupClientOptions);

				return _lookupClient;
			}
		}

		public Lookup() { }

		public Lookup(TimeSpan? minimumTtl, TimeSpan? maximumTtl = null)
			: this()
		{
			this.MinimumTtl = minimumTtl;

			if (maximumTtl != null)
				this.MaximumTtl = maximumTtl;
		}

		public async Task<string> QueryAsync(string fqdn, QueryType queryType)
		{
			string result = string.Empty;

			var dnsQueryResponse = await this.LookupClient.QueryAsync(fqdn, queryType);

			var answer = dnsQueryResponse?.Answers?[0];

			if (answer != null)
			{
				if (answer.RecordType == DnsClient.Protocol.ResourceRecordType.CNAME)
					result = (answer as CNameRecord)?.CanonicalName?.Value;
				else if (answer.RecordType == ResourceRecordType.A)
					result = (answer as AddressRecord)?.Address?.ToString();
				else if (answer.RecordType == ResourceRecordType.AAAA)
					result = (answer as DnsClient.Protocol.AaaaRecord)?.Address?.ToString();
				else if (answer.RecordType == ResourceRecordType.MX)
					result = (answer as DnsClient.Protocol.MxRecord)?.Exchange?.Value;
				else if (answer.RecordType == ResourceRecordType.NS)
					result = (answer as DnsClient.Protocol.NsRecord)?.NSDName?.Value;
				else if (answer.RecordType == ResourceRecordType.PTR)
					result = (answer as DnsClient.Protocol.PtrRecord)?.PtrDomainName?.Value;
				else if (answer.RecordType == ResourceRecordType.SOA)
					result = (answer as DnsClient.Protocol.SoaRecord)?.MName?.Value;
				else
					result = "Not Implemented";
			}

			if (result.EndsWith('.'))
				result = result.Substring(0, result.Length - 1);

			return result;
		}
	}
}
