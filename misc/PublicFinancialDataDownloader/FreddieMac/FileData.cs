using System;
using System.Collections.Generic;
using System.Text;

namespace FreddieMac
{
	public class FileData
	{
		internal string FileName { get; set; }
		internal double FileSizeInBytes { get; set; }
		internal DateTime LastUpdated { get; set; }
	}
}
