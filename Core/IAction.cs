namespace Greatbone.Core
{
    /// <summary>
    /// </summary>
    public interface IAction : IKeyed
    {
        WebSub Controller { get; }

    }
}