using System.IO;
using Newtonsoft.Json;

namespace Greatbone.Core
{
	public class WebServiceBuilder
	{
		public string Key { get; set; }

		public string StaticPath { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public QueueBuilder Queue { get; set; }

		public DataSourceBuilder DataSource { get; set; }

		public bool Debug { get; set; }

		internal IParent Parent { get; set; }

		internal WebService Service { get; set; }


		public static WebServiceBuilder Load(string key)
		{
			string file = Path.GetFileName("_" + key + ".json");
			WebServiceBuilder builder = JsonConvert.DeserializeObject<WebServiceBuilder>(File.ReadAllText(file));
			if (builder.Key == null)
			{
				builder.Key = key;
			}
			return builder;
		}
	}


	public class QueueBuilder
	{
		public string Host { get; set; }

		public int Port { get; set; }
	}

	public class DataSourceBuilder
	{
		public string Host { get; set; }

		public int Port { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}