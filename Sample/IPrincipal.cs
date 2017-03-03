namespace Greatbone.Sample
{
    public interface IPrincipal
    {
        Token ToToken();

        string Credential { get; }
    }
}