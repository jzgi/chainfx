using Greatbone.Core;

namespace Greatbone.Sample
{
	public static class WebContextUtility
	{
		public static bool CheckOwn(this WebContext wc, string x)
		{
			IToken df = wc.Token;
			return false;
		}
	}
}