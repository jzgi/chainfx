namespace Greatbone.Core
{
    ///
    /// A data object that follows certain input/ouput paradigm.
    ///
    public interface IData
    {
        void ReadData(IDataInput i, ushort sel = 0);

        void WriteData<R>(IDataOutput<R> o, ushort sel = 0) where R : IDataOutput<R>;
    }
}
