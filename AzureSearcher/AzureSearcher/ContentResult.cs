using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AzureSearchTester
{
	public class ContentResult
	{
		private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };

		[JsonIgnore]
		public string Content { get; set; }

		public string BlobKey { get; set; }
		public int Size { get; set; }
		public string ContentMD5 { get; set; }
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public string Author { get; set; }

		public DateTime DateFileCreated { get; set; }
		public DateTime DateFileLastModified { get; set; }
		public DateTime DateFileLastModifiedInAzure { get; set; }

		public List<string> CognitiveLocations { get; set; } = new List<string>();

		public List<string> Addresses { get; set; } = new List<string>();

		
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, _jsonSettings);
		}
	}
}
