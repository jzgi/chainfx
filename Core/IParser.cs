namespace Greatbone.Core
{
    public interface IParser<T> where T : IDataInput
    {
        T Parse();
    }
}