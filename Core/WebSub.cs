using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
	///
	/// Represents a (sub)controller that consists of a group of action methods, as well as a folder of static files.
	///
	public abstract class WebSub : IMember
	{
		// the collection of actions declared by this sub-controller
		readonly Set<WebAction> actions;

		// the default action
		readonly WebAction defaction;


		public Checker Checker { get; internal set; }

		// the argument makes state-passing more convenient
		protected WebSub(WebBuilder builder)
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
				Statics = new Set<StaticContent>(256);
				foreach (string path in Directory.GetFiles(StaticPath))
				{
					string file = Path.GetFileName(path);
					string ext = Path.GetExtension(path);
					string ctype;
					if (StaticContent.TryGetType(ext, out ctype))
					{
						byte[] content = File.ReadAllBytes(path);
						DateTime modified = File.GetLastWriteTime(path);
						StaticContent sta = new StaticContent
						{
							Key = file.ToLower(),
							Type = ctype,
							Buffer = content,
							LastModified = modified
						};
						Statics.Add(sta);
						if (sta.Key.StartsWith("index."))
						{
							DefaultStatic = sta;
						}
					}
				}
			}

			actions = new Set<WebAction>(32);

			Type type = GetType();

			// introspect action methods
			foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
			{
				ParameterInfo[] pis = mi.GetParameters();
				if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
				{
					WebAction a = new WebAction(this, mi, false);
					if (a.Key.Equals("Default"))
					{
						defaction = a;
					}
					actions.Add(a);
				}
			}
		}

		///
		/// The key by which this sub-controller is added to its parent
		///
		public string Key { get; }

		public string StaticPath { get; }

		///
		/// The corresponding static folder contents, can be null
		///
		public Set<StaticContent> Statics { get; }

		///
		/// The index static file of the controller, can be null
		///
		public StaticContent DefaultStatic { get; }

		///
		/// The parent service that this sub-controller is added to
		///
		public WebSub Parent { get; }

		///
		/// The service that this component resides in.
		///
		public WebService Service { get; }


		public WebAction GetAction(String action)
		{
			if (string.IsNullOrEmpty(action))
			{
				return defaction;
			}
			return actions[action];
		}

		public virtual void Handle(string relative, WebContext wc)
		{
			if (relative.IndexOf('.') != -1) // static handling
			{
				StaticContent sta;
				if (Statics != null && Statics.TryGet(relative, out sta))
				{
					wc.Response.Content = sta;
				}
				else
				{
					wc.Response.StatusCode = 404;
				}
			}
			else
			{
				// action handling
				WebAction a = relative.Length == 0 ? defaction : GetAction(relative);
				if (a == null)
				{
					wc.Response.StatusCode = 404;
				}
				else
				{
					a.Do(wc);
				}
			}
		}

		public virtual void Default(WebContext wc)
		{
			StaticContent sta = DefaultStatic;
			if (sta != null)
			{
				wc.Response.Content = sta;
			}
			else
			{
				// send not implemented
				wc.Response.StatusCode = 404;
			}
		}

		public virtual void Default(WebContext wc, string x)
		{
			StaticContent sta = DefaultStatic;
			if (sta != null)
			{
				wc.Response.Content = sta;
			}
			else
			{
				// send not implemented
				wc.Response.StatusCode = 404;
			}
		}
	}
}