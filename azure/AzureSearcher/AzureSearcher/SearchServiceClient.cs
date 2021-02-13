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
	public class SearchServiceClient
	{
		private static SearchIndexClient _searchIndexClient = null;

		public string SearchServiceName { get; private set; }
		public string SearchServiceQueryApiKey { get; private set; }
		public string IndexName { get; private set; }

		public SearchIndexClient SearchIndexClient
		{
			get
			{
				if (_searchIndexClient == null)
					_searchIndexClient = new SearchIndexClient(this.SearchServiceName, this.IndexName, new SearchCredentials(this.SearchServiceQueryApiKey));

				return _searchIndexClient;
			}
		}


		private SearchServiceClient() { }

		public SearchServiceClient(string searchServiceName, string searchServiceQueryApiKey, string indexName)
		{
			this.SearchServiceName = searchServiceName;
			this.SearchServiceQueryApiKey = searchServiceQueryApiKey;
			this.IndexName = indexName;
		}


		public async Task<DocumentSearchResult> Search(string searchText, string filterText)
		{
			if (string.IsNullOrWhiteSpace(searchText))
				searchText = "*";

			SearchParameters searchParams = null;

			if (!string.IsNullOrWhiteSpace(filterText))
				searchParams = new SearchParameters() { Filter = filterText };

			DocumentSearchResult documentSearchResult = await this.SearchIndexClient.Documents.SearchAsync(searchText, searchParams);

			return documentSearchResult;
		}
	}
}
