namespace Greatbone.Core
{
    public interface IToken : IPersist
    {
        string Key { get; }

        string Name { get; }

        string[] Roles { get; }
    }
}