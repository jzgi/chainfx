namespace ChainBase
{
    /// <summary>
    /// A data object that reads from input source and writes to output sink.
    /// </summary>
    public interface IData
    {
        void Read(ISource s, byte proj = 0x0f);

        void Write(ISink s, byte proj = 0x0f);
    }
}