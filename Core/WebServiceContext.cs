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

		// public socket address
		internal string @public;

		internal bool tls;

		// private socket address
		internal string @private;

		// event system socket addresses
		internal string[] peers;

		internal DataSrcBuilder DataSrc;

		internal bool debug;

		internal Dictionary<string, string> options;

		// CONTEXT
		//

		internal WebService Service { get; set; }

		internal Stack<WebSub> Chain { get; } = new Stack<WebSub>(8);

		public void ReadFrom(ISerialReader r)
		{
			r.Read(nameof(key), ref key);
			r.Read(nameof(@public), ref @public);
			r.Read(nameof(tls), ref tls);
			r.Read(nameof(@private), ref @private);
			r.Read(nameof(peers), ref peers);
			r.Read(nameof(DataSrc), ref DataSrc);
			r.Read(nameof(debug), ref debug);
			r.Read(nameof(options), ref options);
		}

		public void WriteTo(ISerialWriter w)
		{
			w.Write(nameof(key), key);
			w.Write(nameof(@public), @public);
			w.Write(nameof(tls), tls);
			w.Write(nameof(@private), @private);
			w.Write(nameof(peers), peers);
			w.Write(nameof(DataSrc), DataSrc);
			w.Write(nameof(debug), debug);
			w.Write(nameof(options), options);
		}

		public WebServiceContext Load(string file)
		{
			string json = File.ReadAllText(file);

			JsonText text = new JsonText(json);

			text.ReadLeft();
			ReadFrom(text);
			text.ReadRight();

			if (key == null)
			{
				key = Path.GetFileNameWithoutExtension(file);
			}
			return this;
		}
	}

	public class DataSrcBuilder : ISerial
	{
		internal string host;

		internal int port;

		internal string username;

		internal string password;

		public void ReadFrom(ISerialReader r)
		{
			r.Read(nameof(host), ref host);
			r.Read(nameof(port), ref port);
			r.Read(nameof(username), ref username);
			r.Read(nameof(password), ref password);
		}

		public void WriteTo(ISerialWriter w)
		{
			w.Write(nameof(host), host);
			w.Write(nameof(port), port);
			w.Write(nameof(username), username);
			w.Write(nameof(password), password);
		}
	}
}