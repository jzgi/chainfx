namespace Greatbone.Core
{
    /// <summary>
    /// Represents a data object that is compliant to standard data exchange mechanisms.
    /// </summary>
    public interface IData
    {
        void Read(IIn i);

        void Write(IOut o);
    }
}