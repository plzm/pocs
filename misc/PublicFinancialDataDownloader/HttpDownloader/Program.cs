using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using FreddieMac;

namespace HttpDownloader
{
	class Program
	{
		static void Main(string[] args)
		{
			new Downloader().Download().Wait();
		}
	}
}
