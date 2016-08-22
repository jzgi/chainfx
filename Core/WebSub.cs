using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
	///
	/// Represents a (sub)controller that consists of a group of action methods, as well as a folder of static files.
	///
	public abstract class WebSub : WebController
	{
		// the collection of actions declared by this sub-controller
		readonly Set<WebAction> actions;

		// the default action
		readonly WebAction defaction;


		public Checker Checker { get; internal set; }

		// the argument makes state-passing more convenient
		protected WebSub(WebBuilder builder) : base(builder)
		{

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



	///
	/// Represents a multiplexed sub-controller that consists of a group of action methods.
	///
	public abstract class WebSub<TX> : WebController where TX : IComparable<TX> , IEquatable<TX>
	{
		// the collection of multiplexed actions declared by this sub-controller
		private readonly Set<WebAction<TX>> _actions = new Set<WebAction<TX>>(32);

		// the default action
		private readonly WebAction<TX> _defaction;

		// the argument makes state-passing more convenient
		protected WebSub(WebBuilder builder) : base(builder)
		{
			Type type = GetType();

			// introspect action methods
			foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
			{
				ParameterInfo[] pis = mi.GetParameters();
				if (pis.Length == 2 &&
				    pis[0].ParameterType == typeof(WebContext) &&
				    pis[0].ParameterType == typeof(TX))
				{
					WebAction<TX> a = new WebAction<TX>(this, mi);
					if (a.Key.Equals("Default"))
					{
						_defaction = a;
					}
					_actions.Add(a);
				}
			}
		}


		public Checker<TX> Checker { get; internal set; }

		public WebAction<TX> GetAction(string action)
		{
			return _actions[action];
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
				WebAction<TX> a = relative.Length == 0 ? _defaction : GetAction(relative);
				if (a == null)
				{
					wc.Response.StatusCode = 404;
				}
				else
				{
					a.Do(wc, (TX) (wc.X));
				}
			}
		}

		public virtual void Default(WebContext wc, TX unit)
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