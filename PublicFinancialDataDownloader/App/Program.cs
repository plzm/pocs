using System;
using FreddieMac;

namespace App
{
	class Program
	{
		static void Main(string[] args)
		{
			new Downloader().Download().Wait();
		}
	}
}
