namespace Greatbone.Core
{
    public interface IResultSet
    {
        bool Get(out int value);

        bool Get(out string value);

        bool Get(string name, out string value);
    }
}