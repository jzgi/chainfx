namespace Greatbone.Core
{
	public interface IToken
	{
		string Login { get; }

		string[] Roles { get; }
	}
}