namespace Greatbone.Core
{
	public interface IResultSet
	{
		bool Got(string name, ref int value);

		bool Got(string name, ref decimal value);

		bool Got(string name, ref string value);
	}
}