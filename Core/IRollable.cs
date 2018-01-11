namespace Greatbone.Core
{
    /// <summary>
    /// An object with an key name so that can be a map element.
    /// </summary>
    public interface IRollable<out K>
    {
        K Key { get; }
    }
}