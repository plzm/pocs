using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pi
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Task<string> t1 = DoAThingAsync();
			Task<Guid> t2 = DoAnotherThingAsync();

			Task t0 = Task.WhenAll(new List<Task>() { t1, t2 });

			t0.Wait();
			Guid foo = new Guid();
			string s1 = t1.Result;
			Guid s2 = t2.Result;
		}

		internal static Task<string> DoAThingAsync()
		{
			return null;
		}

		internal static Task<Guid> DoAnotherThingAsync()
		{
			return null;
		}
	}
}
