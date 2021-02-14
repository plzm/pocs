﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace MessageProducer
{
	public class Sender
	{
		private Dictionary<string, ISenderClient> Clients { get; } = new Dictionary<string, ISenderClient>();

		private T GetClient<T>(string asbNsEndpointFqdn, string sharedAccessKeyName, string sharedAccessKeyValue, string entityName)
			where T : class, ISenderClient
		{
			ISenderClient result = null;

			string connString = $"Endpoint=sb://{asbNsEndpointFqdn}/;SharedAccessKeyName={sharedAccessKeyName};SharedAccessKey={sharedAccessKeyValue}";

			string key = $"{connString};EntityPath={entityName}";

			if (this.Clients.ContainsKey(key))
				result = this.Clients[key];
			else
			{
				if (typeof(T).Equals(typeof(QueueClient)))
					result = new QueueClient(connString, entityName);
				else if (typeof(T).Equals(typeof(TopicClient)))
					result = new TopicClient(connString, entityName);

				if (result != null)
					this.Clients.Add(key, result);
			}

			return result as T;
		}

		public async Task SendQueueMessageAsync(string asbNsEndpointFqdn, string sharedAccessKeyName, string sharedAccessKeyValue, string queueName, string messageBody)
		{
			QueueClient qc = GetClient<QueueClient>(asbNsEndpointFqdn, sharedAccessKeyName, sharedAccessKeyValue, queueName);

			Message message = new Message(Encoding.UTF8.GetBytes(messageBody));

			await qc.SendAsync(message);
		}

		public async Task SendQueueMessagesAsync(string asbNsEndpointFqdn, string sharedAccessKeyName, string sharedAccessKeyValue, string queueName, IEnumerable<string> messageBodies)
		{
			QueueClient qc = GetClient<QueueClient>(asbNsEndpointFqdn, sharedAccessKeyName, sharedAccessKeyValue, queueName);

			IList<Message> messages = messageBodies.Select(m => new Message(Encoding.UTF8.GetBytes(m))).ToList();

			await qc.SendAsync(messages);
		}

		public async Task SendTopicMessageAsync(string asbNsEndpointFqdn, string sharedAccessKeyName, string sharedAccessKeyValue, string topicName, string messageBody)
		{
			TopicClient tc = GetClient<TopicClient>(asbNsEndpointFqdn, sharedAccessKeyName, sharedAccessKeyValue, topicName);

			Message message = new Message(Encoding.UTF8.GetBytes(messageBody));

			await tc.SendAsync(message);
		}

		public async Task SendTopicMessagesAsync(string asbNsEndpointFqdn, string sharedAccessKeyName, string sharedAccessKeyValue, string topicName, IEnumerable<string> messageBodies)
		{
			TopicClient tc = GetClient<TopicClient>(asbNsEndpointFqdn, sharedAccessKeyName, sharedAccessKeyValue, topicName);

			IList<Message> messages = messageBodies.Select(m => new Message(Encoding.UTF8.GetBytes(m))).ToList();

			await tc.SendAsync(messages);
		}
	}
}
