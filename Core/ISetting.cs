namespace Greatbone.Core
{
    public interface ISetting
    {
        string Key { get; }

        bool Authenticate { get; }

        bool IsVar { get; }

        WebSub Parent { get; }

        WebService Service { get; }
    }
}