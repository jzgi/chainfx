namespace Greatbone.Core
{
    public interface ISetting
    {
        string Key { get; }

        bool AuthRequired { get; }

        bool IsVar { get; }

        IParent Parent { get; }

        string Folder { get; }

        WebService Service { get; }
    }
}