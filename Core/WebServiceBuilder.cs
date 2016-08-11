using System.IO;
using Newtonsoft.Json;

namespace Greatbone.Core
{
	public class WebServiceBuilder : ISerial
	{
		public string Key;

		public string StaticPath;

		public string Host;

		public int Port;

		public QueueBuilder Queue;

		public DataSourceBuilder DataSource;

		public bool Debug;

		internal IParent Parent;

		internal WebService Service;


		public void From(IReader r)
		{
			r.Read(nameof(Key), ref Key);
			r.Read(nameof(Host), ref Host);
			r.Read(nameof(Queue), ref Queue);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(Key), Key);
			w.Write(nameof(Host), Host);
			w.Write(nameof(Queue), Queue);
		}


		public static WebServiceBuilder Load(string key)
		{
			string file = Path.GetFileName("_" + key + ".json");
			string json = File.ReadAllText(file);

			WebServiceBuilder builder = new JsonText(json).Read<WebServiceBuilder>();
			if (builder.Key == null)
			{
				builder.Key = key;
			}
			return builder;
		}
	}


	public class QueueBuilder : ISerial
	{
		public string Host;

		public int Port;

		public void From(IReader r)
		{
			r.Read(nameof(Host), ref Host);
			r.Read(nameof(Port), ref Port);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(Host), Host);
			w.Write(nameof(Port), Port);
		}
	}

	public class DataSourceBuilder : ISerial
	{
		public string Host;

		public int Port;

		public string Username;

		public string Password;

		public void From(IReader r)
		{
			r.Read(nameof(Host), ref Host);
			r.Read(nameof(Port), ref Port);
			r.Read(nameof(Username), ref Username);
			r.Read(nameof(Password), ref Password);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(Host), Host);
			w.Write(nameof(Port), Port);
			w.Write(nameof(Username), Username);
			w.Write(nameof(Password), Password);
		}
	}
}