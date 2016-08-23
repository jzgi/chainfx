using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
	public class WebServiceBuilder : ISerial
	{
		internal string key;

		internal string host;

		internal string[] evthosts;

		internal DataSrcBuilder datasrc;

		internal bool debug;

		internal Dictionary<string, string> options;

		public void From(IReader r)
		{
			r.Read(nameof(key), ref key);
			r.Read(nameof(host), ref host);
			r.Read(nameof(evthosts), ref evthosts);
			r.Read(nameof(datasrc), ref datasrc);
			r.Read(nameof(debug), ref debug);
			r.Read(nameof(options), ref options);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(key), key);
			w.Write(nameof(host), host);
			w.Write(nameof(evthosts), evthosts);
			w.Write(nameof(datasrc), datasrc);
			w.Write(nameof(debug), debug);
			w.Write(nameof(options), options);
		}

		public static WebServiceBuilder Load(string path)
		{
			string json = File.ReadAllText(path);
			WebServiceBuilder builder = new JsonText(json).Read<WebServiceBuilder>();
			if (builder.key == null)
			{
				string key = Path.GetFileNameWithoutExtension(path);
				builder.key = key;
			}
			return builder;
		}
	}

	public class DataSrcBuilder : ISerial
	{
		internal string host;

		internal int port;

		internal string username;

		internal string password;

		public void From(IReader r)
		{
			r.Read(nameof(host), ref host);
			r.Read(nameof(port), ref port);
			r.Read(nameof(username), ref username);
			r.Read(nameof(password), ref password);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(host), host);
			w.Write(nameof(port), port);
			w.Write(nameof(username), username);
			w.Write(nameof(password), password);
		}
	}
}