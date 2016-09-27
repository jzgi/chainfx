namespace Greatbone.Core
{
    public interface IResultSet : IIn
    {
        bool Get(ref int value);

        bool Get(ref string value);
    }
}