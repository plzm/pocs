using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus.Administration;
using pelazem.http;

namespace CustomMetricsSender
{
	public static class Sender
	{
		#region Variables

		private static AzureServiceTokenProvider _azureServiceTokenProvider = null;
		private static ServiceBusAdministrationClient _asbClient = null;
		private static HttpUtil _httpUtil = new HttpUtil();

		private static string _azureRegion = Environment.GetEnvironmentVariable("AzureRegion");
		private static string _azureSubscriptionId = Environment.GetEnvironmentVariable("AzureSubscriptionId");
		private static string _asbResourceGroup = Environment.GetEnvironmentVariable("AsbResourceGroup");
		private static string _asbNamespaceName = Environment.GetEnvironmentVariable("AsbNamespaceName");
		private static string _asbConnectionString = Environment.GetEnvironmentVariable("AsbConnectionString");
		private static string _asbTopicName = Environment.GetEnvironmentVariable("AsbTopicName");
		private static string _asbSubscriptionName = Environment.GetEnvironmentVariable("AsbSubscriptionName");
		private static string _customMetricNamespace = Environment.GetEnvironmentVariable("CustomMetricNamespace");

		private static string _asbResourceId = $"subscriptions/{_azureSubscriptionId}/resourceGroups/{_asbResourceGroup}/providers/Microsoft.ServiceBus/namespaces/{_asbNamespaceName}";
		private static string _monitorApiUrl = $"https://{_azureRegion}.monitoring.azure.com/{_asbResourceId}/metrics";

		private static string _customMetricName = $"{_asbTopicName}-{_asbSubscriptionName}-ActiveMsgCount";

		#endregion

		#region Properties

		private static AzureServiceTokenProvider AzureServiceTokenProvider
		{
			get
			{
				if (_azureServiceTokenProvider == null)
					_azureServiceTokenProvider = new AzureServiceTokenProvider();

				return _azureServiceTokenProvider;
			}
		}

		private static ServiceBusAdministrationClient AsbClient
		{
			get
			{
				if (_asbClient == null)
					_asbClient = new ServiceBusAdministrationClient(_asbConnectionString);

				return _asbClient;
			}
		}

		private static HttpUtil HttpUtil
		{
			get
			{
				if (_httpUtil == null)
					_httpUtil = new HttpUtil();

				return _httpUtil;
			}
		}

		#endregion

		[FunctionName("Sender")]
		public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation("Retrieve subscription active message count");
			long subActMsgCount = GetSubscriptionActiveMessageCountAsync().Result;
			log.LogInformation($"Got subscription active message count = {subActMsgCount}");

			log.LogInformation("Prepare custom metric payload");
			string bodyPayload = GetBodyPayload(subActMsgCount, _customMetricNamespace, _customMetricName);
			log.LogInformation(bodyPayload);

			log.LogInformation("Get bearer token value");
			string accessToken = GetAccessTokenAsync().Result;

			log.LogInformation("Send custom metric payload");
			HttpResponseMessage httpResponseMessage = SendCustomMetricMessageAsync(accessToken, bodyPayload).Result;
			
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				log.LogInformation("SUCCESS!");
			}
			else
			{
				log.LogCritical("You... FAILED.");
				log.LogCritical(httpResponseMessage.ReasonPhrase);
			}
		}

		private static async Task<string> GetAccessTokenAsync()
		{
			// AzureServiceTokenProvider internally caches the token and automatically refreshes it when needed (expiration)
			string accessToken = await AzureServiceTokenProvider.GetAccessTokenAsync("https://monitoring.azure.com/");

			return accessToken;
		}

		private static string GetBodyPayload(long messageCount, string customMetricNamespace, string customMetricName)
		{
			string timestamp = string.Format("{0:O}", DateTime.UtcNow);
			string msgCount = messageCount.ToString();

			string result = String.Join
			(
				Environment.NewLine,
				"{",
				$"\"time\": \"{timestamp}\",",
				"\"data\":",
				"{",
				"\"baseData\":",
				"{",
				$"\"namespace\": \"{customMetricNamespace}\",",
				$"\"metric\": \"{customMetricName}\",",
				"\"dimNames\":",
				"[",
				"\"Count\"",
				"],",
				"\"series\":",
				"[",
				"{",
				"\"dimValues\":",
				"[",
				"\"Active\"",
				"],",
				$"\"min\": {msgCount},",
				$"\"max\": {msgCount},",
				$"\"sum\": {msgCount},",
				"\"count\": 1",
				"}",
				"]",
				"}",
				"}",
				"}"
			);

			return result;
		}

		private static async Task<long> GetSubscriptionActiveMessageCountAsync()
		{
			SubscriptionRuntimeProperties subRunTimeProps = await AsbClient.GetSubscriptionRuntimePropertiesAsync(_asbTopicName, _asbSubscriptionName);

			return subRunTimeProps.ActiveMessageCount;
		}

		private static async Task<HttpResponseMessage> SendCustomMetricMessageAsync(string accessToken, string bodyPayload)
		{
			HttpUtil.AddRequestHeader("Authorization", $"Bearer {accessToken}");

			HttpContent httpContent = HttpUtil.PrepareHttpContent(bodyPayload, "application/json");

			HttpResponseMessage httpResponseMessage = await HttpUtil.PostAsync(_monitorApiUrl, httpContent);

			return httpResponseMessage;
		}
	}
}
