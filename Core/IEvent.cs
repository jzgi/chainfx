namespace Greatbone.Core
{
    /// <summary>
    /// A data record that follows certain input/ouput paradigm.
    /// </summary>
    public interface IEvent
    {
        int Seq { get; }
    }
}