using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace KeyVaultTester
{
	class Program
	{
		static void Main(string[] args)
		{
			string keyVaultRoot = "https://PROVIDE.vault.azure.net/secrets/";

			var azureServiceTokenProvider = new AzureServiceTokenProvider();

			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

			SecretBundle secretBundle = keyVaultClient.GetSecretAsync($"{keyVaultRoot}Secret1").Result;

			var secretValue = secretBundle.Value;
		}
	}
}
