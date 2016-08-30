using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class AdminAttribute : Attribute, IChecker
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