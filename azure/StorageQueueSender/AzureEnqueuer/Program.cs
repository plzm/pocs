using System;
using System.Threading.Tasks;

namespace AzureQueueTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Enqueuer q = new Enqueuer();

			q.Enqueue("Hello world 3").Wait();

			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
