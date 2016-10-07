namespace Greatbone.Core
{
    public interface ITie
    {
        string Key { get; }

        bool IsVar { get; }

        WebSub Parent { get; }

        WebService Service { get; }
    }
}