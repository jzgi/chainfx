namespace Greatbone.Core
{
    public interface IResultSet
    {
        bool Get(ref int value);

        bool Get(ref string value);

        bool Get(string name, ref short value);

        bool Get(string name, ref int value);

        bool Get(string name, ref string value);
    }
}