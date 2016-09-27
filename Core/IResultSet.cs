namespace Greatbone.Core
{
    public interface IResultSet : IInput
    {
        bool Get(ref int value);

        bool Get(ref string value);
    }
}