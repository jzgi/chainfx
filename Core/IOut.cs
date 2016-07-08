namespace Greatbone.Core
{
    public interface IOut
    {
        string ContentType { get; }

        byte[] Buffer { get; }

        int Offset { get; }

        int Count { get; }
    }
}