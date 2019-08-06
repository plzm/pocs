using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonManipulator
{
	class Program
	{
		private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include, Formatting = Formatting.None };

		static void Main(string[] args)
		{
			string msg = GetTripMessage();

			Console.WriteLine(msg);
			Console.WriteLine();

			JObject jo = JObject.Parse(msg);
			Console.WriteLine(jo["customer_sentiment"].ToString());
			Console.WriteLine(jo["customer_comments"].ToString());

			jo["customer_comments-ta"] = "foo";

			Console.WriteLine(jo.ToString());

			Console.WriteLine();
			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}

		private static string GetTripMessage()
		{
			TripMessage message = new TripMessage();

			message.trip_type = 1;
			message.trip_year = DateTime.Now.Year.ToString();
			message.trip_month = string.Format("{0:MM}", DateTime.Now);
			message.taxi_type = "Yellow";
			message.vendor_id = 1;
			message.pickup_datetime = DateTime.Now.AddMinutes(-30);
			message.dropoff_datetime = message.pickup_datetime.AddMinutes(15);
			message.passenger_count = 2;
			message.trip_distance = 5;
			message.rate_code_id = 1;
			message.store_and_fwd_flag = "";
			message.pickup_location_id = 66;
			message.dropoff_location_id = 99;
			message.pickup_longitude = "77.7777";
			message.pickup_latitude = "33.3333";
			message.dropoff_longitude = "77.9999";
			message.dropoff_latitude = "33.6666";
			message.payment_type = 2;
			message.fare_amount = 13;
			message.extra = 1.5;
			message.mta_tax = 2.2;
			message.tip_amount = 3;
			message.tolls_amount = 1.75;
			message.improvement_surcharge = 0.6;
			message.ehail_fee = 0.99;

			message.customer_sentiment = 2;
			message.customer_comments = "The trip took too long. The taxi was uncomfortable: it was too cold, the seats were dirty, and the car smelled bad. The driver was a purple two-headed alien with a spitting problem, which was mildly disconcerting to say the least! Overall it was gross.";

			return JsonConvert.SerializeObject(message, _jsonSerializerSettings);
		}
	}
}
