namespace Greatbone.Core
{
	public interface IContent
	{
		string Type { get; }

		byte[] Buffer { get; }

		int Offset { get; }

		int Count { get; }
	}
}