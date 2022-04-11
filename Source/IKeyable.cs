namespace Chainly
{
    /// <summary>
    /// An object with an key name so that can be a map element.
    /// </summary>
    public interface IKeyable<out K>
    {
        K Key { get; }
    }
}