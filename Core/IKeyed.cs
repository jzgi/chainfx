namespace Greatbone.Core
{
    ///
    /// An obect that is identified by string key thus can be a number of Roll.
    ///
    public interface IKeyed
    {
        string Key { get; }
    }
}