using System;
using System.IO;
using Npgsql;

namespace Greatbone.Core
{
	///
	/// Represents an abstract controller.
	///
	public abstract class WebController : IMember
	{
		///
		/// The key by which this sub-controller is added to its parent
		///
		public string Key { get; }

		public string StaticPath { get; }

		///
		/// The corresponding static folder contents, can be null
		///
		public Set<Static> Statics { get; }

		///
		/// The index static file of the controller, can be null
		///
		public Static IndexStatic { get; }

		///
		/// The parent service that this sub-controller is added to
		///
		public IParent Parent { get; }

		///
		/// The service that this component resides in.
		///
		public WebService Service { get; }


		internal WebController(WebServiceBuilder builder)
		{
			if (builder.Key == null)
			{
				throw new ArgumentNullException(nameof(builder.Key));
			}

			Key = builder.Key;
			Parent = builder.Parent;
			Service = builder.Service;
			StaticPath = builder.StaticPath ?? Path.Combine(Directory.GetCurrentDirectory(), "_" + Key);

			// load static files, if any
			if (StaticPath != null && Directory.Exists(StaticPath))
			{
				Statics = new Set<Static>(256);
				foreach (string path in Directory.GetFiles(StaticPath))
				{
					string file = Path.GetFileName(path);
					string ext = Path.GetExtension(path);
					string ctype;
					if (Static.TryGetType(ext, out ctype))
					{
						byte[] content = File.ReadAllBytes(path);
						DateTime modified = File.GetLastWriteTime(path);
						Static sta = new Static
						{
							Key = file.ToLower(),
							Type = ctype,
							Buffer = content,
							Modified = modified
						};
						Statics.Add(sta);
						if (sta.Key.StartsWith("index."))
						{
							IndexStatic = sta;
						}
					}
				}
			}
		}

		public NpgsqlContext NewSqlContext()
		{
			WebService svc = Service;
			NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
			{
				Host = "localhost",
				Database = svc.Key,
				Username = "postgres",
				Password = "Zou###1989"
			};
			return new NpgsqlContext(builder);
		}
	}
}