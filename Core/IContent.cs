namespace Greatbone.Core
{
    public interface IContent
    {
        string ContentType { get; }

        byte[] Buffer { get; }

        int Offset { get; }

        int Count { get; }
    }
}