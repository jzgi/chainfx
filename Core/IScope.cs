namespace Greatbone.Core
{
    public interface IScope
    {
        string Key { get; }

        bool IsVar { get; }

        WebSub Parent { get; }

        WebService Service { get; }
    }
}