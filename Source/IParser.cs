namespace SkyCloud
{
    /// <summary>
    /// A data structure that is parser for a certain content format.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParser<out T> where T : ISource
    {
        T Parse();
    }
}