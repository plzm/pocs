using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
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
		private static string _endpointUrl = "";
		private static string _key = "";
		private static string _databaseId = "";
		private static string _containerId = "";

		public static async Task Main(string[] args)
		{
			IEnumerable<JObject> documents = await GetDocuments(_endpointUrl, _key, _databaseId, _containerId).ConfigureAwait(false);

			IEnumerable<dynamic> results = ProcessDocuments(documents);

			foreach (JObject document in documents)
			{
				string djson = document.ToString();
				//string djson = JsonConvert.SerializeObject(document);

				Console.WriteLine(djson);
				Console.WriteLine();
			}

			foreach (dynamic result in results)
			{
				Console.WriteLine(result.id);
			}

			// OK, we have our results as an IEnumerable<dynamic>... now let's do something useful with the results.
			// Here I'm just sending each to console with a set of key=value pairs so we can see what the dynamic contains.
			//foreach (dynamic result in results)
			//	Console.WriteLine((result as IDictionary<string, object>)?.GetDelimitedList(" | ", "Doh! Nothing there!"));

			Console.WriteLine();
			Console.WriteLine("Done - hit any key to exit.");
			Console.ReadKey();
		}

		private static async Task<IEnumerable<JObject>> GetDocuments(string endpointUrl, string key, string databaseId, string containerId)
		{
			List<JObject> results = new List<JObject>();

			using (CosmosClient client = new CosmosClient(endpointUrl, key))
			{
				Database cosmosDatabase = client.GetDatabase(databaseId);
				Container container = cosmosDatabase.GetContainer(containerId);

				FeedIterator<JObject> query = container.GetItemQueryIterator<JObject>();

				try
				{
					while (query.HasMoreResults)
					{
						FeedResponse<JObject> currentResultSet = await query.ReadNextAsync().ConfigureAwait(false);

						foreach (JObject result in currentResultSet)
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

		//private static string GetQueryText(IEnumerable<string> outputPropertyNames = null)
		//{
		//	// Prepare query text in case we want to project Azure-side
		//	string queryText = null;

		//	if (outputPropertyNames != null && outputPropertyNames.Count() > 0)
		//	{
		//		// Project Azure-side and pass less data back over the wire
		//		string projection = outputPropertyNames.Select(opn => $"c.{opn}").GetDelimitedList(", ", string.Empty, false);

		//		queryText = $"SELECT {projection} FROM c";
		//	}
		//	else
		//	{
		//		// No Azure-side projection - we'll just retrieve entire documents
		//		queryText = null;
		//	}

		//	return queryText;
		//}

		private static IEnumerable<dynamic> ProcessDocuments(IEnumerable<JObject> documents)
		{
			List<dynamic> results = new List<dynamic>();

			foreach (JObject document in documents)
			{
				IEnumerable<JProperty> props = document.Properties().Where(p => !(p.Name.StartsWith("_", StringComparison.InvariantCultureIgnoreCase)));

				foreach (JProperty prop in props)
				{
					if (prop.Value.HasValues)
					{
						// It's a JArray
						JArray array = new JArray(prop.Value);

						Console.WriteLine("Array: " + prop.Name + " | " + prop.Count.ToString());
					}
					else
					{
						// It's a scalar

						Console.WriteLine("Scalar: " + prop.Name + " = " + prop.Value.ToString());
					}
				}

				results.Add(document as dynamic);

				//dynamic result = new ExpandoObject();
				//IDictionary<string, object> resultAsDictionary = result as IDictionary<string, object>;

				//IJEnumerable<JToken> jvalues = document.Values();

				//foreach (JToken jToken in jvalues)
				//	resultAsDictionary.Add
				//	(
				//		jToken.Path,
				//		(jToken.HasValues ? jToken.First.Value<string>("_value") : jToken.Value<string>())
				//	);

				//yield return result;
			}

			return results;
		}
	}
}
