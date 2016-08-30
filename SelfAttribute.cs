using System;
using Greatbone.Core;

namespace Greatbone
{
	public class SelfAttribute : Attribute, IChecker
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