namespace Greatbone.Core
{
    public interface IToken
    {
        string Key { get; }

        string Name { get; }

        string[] Roles { get; }
    }
}