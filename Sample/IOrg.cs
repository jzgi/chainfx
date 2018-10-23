namespace Samp
{
    public interface IOrg
    {
        short Key { get; }

        string Name { get; }
    }

    /// <summary>
    /// For easy sharing code between works.
    /// </summary>
    public interface IOrgVar
    {
    }
}