using System;
using Greatbone.Core;

namespace Greatbone
{
	public class ToSelfAttribute : Attribute, IChecker
	{
		public bool Check(WebContext wc)
		{
			throw new NotImplementedException();
		}

		public bool Check(WebContext wc, string x)
		{
			return false;
		}
	}
}