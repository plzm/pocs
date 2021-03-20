using System;

namespace AzureSearchTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Searcher searcher = new Searcher();

			searcher.Search().Wait();
		}
	}
}
