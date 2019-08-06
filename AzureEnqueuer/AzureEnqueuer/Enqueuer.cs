using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureQueueTester
{
	public class Enqueuer
	{
		private string _storageAccountName = "";
		private string _storageAccountKey = "";

		private string _queueName = "";

		public async Task Enqueue(string message)
		{
			StorageCredentials storageCredentials = new StorageCredentials(_storageAccountName, _storageAccountKey);

			CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

			CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

			CloudQueue queue = queueClient.GetQueueReference(_queueName);

			await queue.CreateIfNotExistsAsync();

			//await queue.AddMessageAsync(new CloudQueueMessage(message), null, null, null, null);

			await queue.AddMessageAsync(new CloudQueueMessage(message), null, TimeSpan.FromMinutes(1), null, null);
		}
	}
}
