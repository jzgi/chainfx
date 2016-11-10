namespace Greatbone.Core
{
    ///
    /// A client principal such as a token or a login identity.
    ///
    public interface IPrincipal
    {
        string Key { get; }

        string Name { get; }

        string Credential { get; }
    }
}