using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using pelazem.util;

namespace AzureSearchTester
{
	public class Searcher
	{
		public async Task Search()
		{
			string serviceName = "az-srch-fpp-poc";
			string serviceApiKey = "28BE896B5A0D057E611407246C8B823A";
			string indexName = "az-srch-fpp-poc-index";

			string filter = "BlobKey eq 'RC9HZW5lcmFsL0Jyb29rd29vZF9TZWxmX1N0b3JhZ2VfTEEtTVNfUG9ydGZvbGlvLnhsc3g'";

			SearchServiceClient client = new SearchServiceClient(serviceName, serviceApiKey, indexName);

			DocumentSearchResult documentSearchResult = await client.Search(string.Empty, filter);

			foreach (SearchResult searchResult in documentSearchResult.Results)
			{
				ContentResult doc = new ContentResult();

				doc.Content = searchResult.Document["content"].ToString();
				doc.BlobKey = searchResult.Document["BlobKey"].ToString();
				doc.Size = Converter.GetInt32(searchResult.Document["metadata_storage_size"]);
				doc.ContentMD5 = searchResult.Document["metadata_storage_content_md5"].ToString();
				doc.FileName = searchResult.Document["metadata_storage_name"].ToString();
				doc.FilePath = searchResult.Document["metadata_storage_path"].ToString();
				doc.Author = searchResult.Document["metadata_author"].ToString();
				doc.DateFileCreated = Converter.GetDateTime(searchResult.Document["metadata_author"]);
				doc.DateFileLastModified = Converter.GetDateTime(searchResult.Document["metadata_last_modified"]);
				doc.DateFileLastModifiedInAzure = Converter.GetDateTime(searchResult.Document["metadata_storage_last_modified"]);
				doc.CognitiveLocations.AddRange(searchResult.Document["locations"] as IEnumerable<string>);

				result.Add(doc);
			}

		}
	}
}
