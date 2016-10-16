namespace Greatbone.Core
{
    public interface ISetting
    {
        string Key { get; }

        bool Authen { get; }

        bool IsVar { get; }

        IParent Parent { get; }

        WebService Service { get; }
    }
}