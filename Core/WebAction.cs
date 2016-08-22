using System.Reflection;

namespace Greatbone.Core
{
	/// <summary>
	/// An action method.
	/// </summary>
	/// <param name="wc"></param>
	public delegate void Doer(WebContext wc);

	public delegate void XDoer(WebContext wc, string x);

	///
	///
	public class WebAction : IMember
	{
		readonly Doer doer;

		readonly XDoer xdoer;

		public WebSub Controller { get; }

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