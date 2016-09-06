namespace Greatbone.Core
{
    public interface IResultSet
    {
        bool Get(ref int value);

        bool Get(ref string value);

        bool Get(string name, ref string value);
    }
}