using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace CustomMetricsSender
{
	public class CustomMetric
	{
		[JsonProperty("time")]
		public string Time { get; set; } = string.Format("O", DateTime.UtcNow);
	}
}
