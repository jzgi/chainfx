namespace DoChain
{
    /// <summary>
    /// A data object that reads from input source and writes to output sink.
    /// </summary>
    public interface IData
    {
        void Read(ISource s, short msk = 0xff);

        void Write(ISink s, short msk = 0xff);
    }
}