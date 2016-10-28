namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A principal can be a token or a login.
    /// </summary>
    public interface IPrincipal
    {
        string Key { get; }

        string Name { get; }

        string Credential { get; }
    }
}