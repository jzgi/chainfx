using System.Reflection;

namespace Greatbone.Core
{
	/// <summary>The delegate of action methods.</summary>
	public delegate void Doer(WebContext wc);

	/// <summary>The delegate of mux action methods.</summary>
	public delegate void XDoer(WebContext wc, string x);

	/// <summary>The descriptor of an action handling method.</summary>
	///
	public class WebAction : IMember
	{
		public WebSub Controller { get; }

		readonly Doer doer;

		readonly XDoer xdoer;

		public string Key { get; }

		public bool IsX { get; }

		internal WebAction(WebSub controller, MethodInfo mi, bool x)
		{
			Controller = controller;
			// NOTE: strict method name as key here to avoid the default base url trap
			Key = mi.Name;
			IsX = x;
			if (x)
			{
				xdoer = (XDoer) mi.CreateDelegate(typeof(XDoer), controller);
			}
			else
			{
				doer = (Doer) mi.CreateDelegate(typeof(Doer), controller);
			}
		}

		internal void Do(WebContext wc)
		{
			doer(wc);
		}

		internal void Do(WebContext wc, string x)
		{
			xdoer(wc, x);
		}

		public override string ToString()
		{
			return Key;
		}
	}
}