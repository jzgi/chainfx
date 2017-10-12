namespace Greatbone.Core
{
    /// <summary>
    /// To report the current state of an data object.
    /// </summary>
    public interface IStatable
    {
        short GetState();
    }
}