namespace Greatbone.Core
{
	public interface IChecker
	{
		bool Check(WebContext wc);

		bool Check(WebContext wc, string x);
	}
}