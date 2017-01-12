namespace Greatbone.Core
{
    ///
    /// A client principal such as a token or a login identity.
    ///
    public interface IToken : IDat
    {
        string Key { get; }

        string Name { get; }
    }
}