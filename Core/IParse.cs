namespace Greatbone.Core
{
    public interface IParse<T> where T : IDataInput
    {
        T Parse();
    }
}