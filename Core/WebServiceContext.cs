using System.Collections.Generic;
using System.IO;
using System;

namespace Greatbone.Core
{
	/// <summary>The configurative settings and the establishment of creation context during initialization of the controller hierarchy.</summary>
	/// <remarks>It provides a strong semantic that enables the whole controller hierarchy to be established within execution of constructors, starting from the constructor of a service controller.</remarks>
	/// <example>
	/// public class FooService : WebService
	/// {
	///         public FooService(WebServiceContext wsc) : base(wsc)
	///         {
	///                 AddSub&lt;BarSub&gt;();
	///         }
	/// }
	/// </example>
	///
	public class WebServiceContext : ISerial
	{
		// SETTINGS
		//

		internal string key;

		internal string host;

		internal string[] evthosts;

		internal DataSrcBuilder datasrc;

		internal bool debug;

		internal Dictionary<string, string> options;

		// CONTEXT
		//

		internal WebService Service { get; set; }

		internal Stack<WebSub> Chain { get; } = new Stack<WebSub>(8);

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

		public static WebServiceContext Load(string file)
		{
			string json = File.ReadAllText(file);
			WebServiceContext context = new JsonText(json).Read<WebServiceContext>();
			if (context.key == null)
			{
				string key = Path.GetFileNameWithoutExtension(file);
				context.key = key;
			}
			return context;
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