namespace Greatbone.Core
{
    public interface IParse<out T> where T : IDataInput
    {
        T Parse();
    }
}