using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace FreddieMac
{
	public class Downloader
	{
		private const string SESSIONCOOKIENAME = "PHPSESSID";

		private string _userName = "";
		private string _password = "";

		private CookieContainer _cookieContainer = null;
		private HttpClientHandler _httpClientHandler = null;
		private HttpClient _httpClient = null;

		private Uri _baseUri = new Uri("https://freddiemac.embs.com");

		private CookieContainer CookieContainer
		{
			get
			{
				if (_cookieContainer == null)
				{
					_cookieContainer = new CookieContainer();
				}

				return _cookieContainer;
			}
		}

		private HttpClientHandler HttpClientHandler
		{
			get
			{
				if (_httpClientHandler == null)
				{
					_httpClientHandler = new HttpClientHandler();

					_httpClientHandler.CookieContainer = this.CookieContainer;
				}

				return _httpClientHandler;
			}
		}

		private HttpClient HttpClient
		{
			get
			{
				if (_httpClient == null)
					_httpClient = new HttpClient(this.HttpClientHandler) { BaseAddress = _baseUri };

				return _httpClient;
			}
		}

		public async Task<bool> Download()
		{
			bool result = false;

			List<FileData> sourceFilesData = await this.GetSourceFilesData();

			foreach (FileData sourceFileData in sourceFilesData)
			{
				// TODO Decide whether to download
				bool downloadIt = true;

				if (downloadIt)
				{
					Uri downloadUri = new Uri(_baseUri.OriginalString + "/FLoan/Data/" + sourceFileData.FileName);

					HttpResponseMessage response = await this.HttpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseContentRead);

					if (response.IsSuccessStatusCode)
					{
						using (MemoryStream memoryStream = new MemoryStream(await response.Content.ReadAsByteArrayAsync(), true))
						{
							using (FileStream file = new FileStream(sourceFileData.FileName, FileMode.Create, FileAccess.Write))
							{
								memoryStream.WriteTo(file);
							}
						}
					}
				}
			}

			return result;
		}

		private async Task<List<FileData>> GetSourceFilesData()
		{
			List<FileData> result = new List<FileData>();

			HttpResponseMessage downloadPageResponse = await this.Login();

			if (downloadPageResponse.IsSuccessStatusCode)
			{
				string responseContent = await downloadPageResponse.Content.ReadAsStringAsync();

				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(responseContent);

				List<HtmlNode> links = doc
					.DocumentNode
					.SelectNodes("//a[@href]")
					.Where(n => n.InnerText.StartsWith("historical_data") && n.InnerText.EndsWith(".zip"))
					.Cast<HtmlNode>()
					.ToList()
				;

				foreach (HtmlNode link in links)
				{
					string fileName = link.InnerText;
					string href = link.Attributes["href"].Value;

					string title = link.Attributes["title"].Value;
					string[] separator = { "bytes Updated" };
					string[] pieces = title.Split(separator, StringSplitOptions.None);
					double sizeInBytes = double.Parse(pieces[0].Trim());
					DateTime updated = DateTime.Parse(pieces[1].Trim());

					result.Add(new FileData() { FileName = fileName, FileSizeInBytes = sizeInBytes, LastUpdated = updated });
				}
			}

			return result;
		}

		private async Task<HttpResponseMessage> Login()
		{
			HttpResponseMessage result = null;

			string requestUri = "/FLoan/secure/auth.php";

			var parameters = new List<KeyValuePair<string, string>>();
			parameters.Add(new KeyValuePair<string, string>("pagename", "download2"));
			parameters.Add(new KeyValuePair<string, string>("username", _userName));
			parameters.Add(new KeyValuePair<string, string>("password", _password));
			var formContent = new FormUrlEncodedContent(parameters);

			HttpResponseMessage response = await this.HttpClient.PostAsync(requestUri, formContent).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				string responseContent = await response.Content.ReadAsStringAsync();

				Cookie sessionCookie = this.CookieContainer.GetCookies(_baseUri).Cast<Cookie>().FirstOrDefault(c => c.Name == SESSIONCOOKIENAME);

				if (sessionCookie != null)
				{
					string sessionCookieValue = sessionCookie.Value;

					result = await this.AcceptTerms();
				}
			}
			else
			{
				result = null;
				// Oh noes, now what
			}

			return result;
		}

		private async Task<HttpResponseMessage> AcceptTerms()
		{
			HttpResponseMessage result = null;

			string requestUri = "/FLoan/Data/download2.php";

			var parameters = new List<KeyValuePair<string, string>>();
			parameters.Add(new KeyValuePair<string, string>("accept", "Yes"));
			parameters.Add(new KeyValuePair<string, string>("action", "acceptTandC"));
			parameters.Add(new KeyValuePair<string, string>("acceptSubmit", "Continue"));
			var formContent = new FormUrlEncodedContent(parameters);

			result = await this.HttpClient.PostAsync(requestUri, formContent).ConfigureAwait(false);

			return result;
		}
	}
}
