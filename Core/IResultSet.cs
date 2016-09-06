namespace Greatbone.Core
{
    public interface IResultSet : ISerialReader
    {
        bool Read(ref int value);

        bool Read(ref string value);
    }
}