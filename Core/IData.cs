namespace Greatbone.Core
{
    ///
    /// A data object that follows certain input/ouput paradigm.
    ///
    public interface IData
    {
        void ReadData(IDataInput i, byte flags = 0);

        void WriteData<R>(IDataOutput<R> o, byte flags = 0) where R : IDataOutput<R>;
    }
}
