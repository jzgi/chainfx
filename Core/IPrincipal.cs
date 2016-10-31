namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A client principal such as a token or a login identity.
    /// </summary>
    public interface IPrincipal
    {
        string Key { get; }

        string Name { get; }

        string Credential { get; }

    }

}