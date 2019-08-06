using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pelazem.util;

namespace CosmicDynamic
{
	class Program
	{
		// SQL or Graph account values
		private static string _endpointUrl = "https://PROVIDE.documents.azure.com:443/";
		private static string _key = "PROVIDE";
		private static string _databaseId = "PROVIDE";
		private static string _containerId = "PROVIDE";

		public static async Task Main(string[] args)
		{
			// What properties do we want to see on the output
			List<string> outputPropertyNames = new List<string>() { "id", "firstname", "lastname", "email" };

			IEnumerable<JObject> documents = await GetDocuments(_endpointUrl, _key, _databaseId, _containerId, outputPropertyNames);

			IEnumerable<dynamic> results = ProcessDocuments(documents);

			// OK, we have our results as an IEnumerable<dynamic>... now let's do something useful with the results.
			// Here I'm just sending each to console with a set of key=value pairs so we can see what the dynamic contains.
			foreach (dynamic result in results)
				Console.WriteLine((result as IDictionary<string, object>)?.GetDelimitedList(" | ", "Doh! Nothing there!"));

			Console.WriteLine();
			Console.WriteLine("Done - hit any key to exit.");
			Console.ReadKey();
		}

		private static async Task<IEnumerable<JObject>> GetDocuments(string endpointUrl, string key, string databaseId, string containerId, IEnumerable<string> outputPropertyNames = null)
		{
			List<JObject> results = new List<JObject>();

			using (CosmosClient client = new CosmosClient(endpointUrl, key))
			{
				Database cosmosDatabase = client.GetDatabase(databaseId);

				Container container = cosmosDatabase.GetContainer(containerId);

				string queryText = GetQueryText(outputPropertyNames);

				FeedIterator<JObject> query = container.GetItemQueryIterator<JObject>(queryText);

				try
				{
					while (query.HasMoreResults)
					{
						foreach (JObject result in await query.ReadNextAsync())
							results.Add(result);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			return results;
		}

		private static string GetQueryText(IEnumerable<string> outputPropertyNames = null)
		{
			// Prepare query text in case we want to project Azure-side
			string queryText = null;

			if (outputPropertyNames != null && outputPropertyNames.Count() > 0)
			{
				// Project Azure-side and pass less data back over the wire
				string projection = outputPropertyNames.Select(opn => $"c.{opn}").GetDelimitedList(", ", string.Empty, false);

				queryText = $"SELECT {projection} FROM c";
			}
			else
			{
				// No Azure-side projection - we'll just retrieve entire documents
				queryText = null;
			}

			return queryText;
		}

		private static IEnumerable<dynamic> ProcessDocuments(IEnumerable<JObject> documents)
		{
			foreach (JObject document in documents)
			{
				dynamic result = new ExpandoObject();
				IDictionary<string, object> resultAsDictionary = result as IDictionary<string, object>;

				foreach (JToken jToken in document.Values())
					resultAsDictionary.Add
					(
						jToken.Path,
						(jToken.HasValues ? jToken.First.Value<string>("_value") : jToken.Value<string>())
					);

				yield return result;
			}
		}
	}
}
