using System;
using System.Reflection;

namespace Greatbone.Core
{
	/// <summary>
	/// An action method.
	/// </summary>
	/// <param name="wc"></param>
	public delegate void Doer(WebContext wc);

	///
	///
	public class WebAction : IMember
	{
		readonly Doer doer;

		public WebSub Controller { get; }

		public string Key { get; }

		internal WebAction(WebSub controller, MethodInfo mi, bool x)
		{
			Controller = controller;
			// NOTE: strict method name as key here to avoid the default base url trap
			Key = mi.Name;
			doer = (Doer) mi.CreateDelegate(typeof(Doer), controller);
		}

		internal void Do(WebContext wc)
		{
			doer(wc);
		}

		public override string ToString()
		{
			return Key;
		}
	}

	public delegate void Doer<in TX>(WebContext wc, TX x) where TX : IComparable<TX>, IEquatable<TX>;

	public class WebAction<TX> : IMember where TX : IComparable<TX>, IEquatable<TX>
	{
		readonly Doer<TX> xdoer;

		public WebSub<TX> Controller { get; }

		public string Key { get; }

		internal WebAction(WebSub<TX> controller, MethodInfo mi)
		{
			Controller = controller;
			// NOTE: strict method name as key here to avoid the default base url trap
			Key = mi.Name;
			xdoer = (Doer<TX>) mi.CreateDelegate(typeof(Doer<TX>), controller);
		}

		internal void Do(WebContext wc, TX x)
		{
			xdoer(wc, x);
		}

		public override string ToString()
		{
			return Key;
		}
	}
}