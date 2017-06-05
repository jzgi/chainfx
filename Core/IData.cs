namespace Greatbone.Core
{
    ///
    /// A data object that follows certain input/ouput paradigm.
    ///
    public interface IData
    {
        void ReadData(IDataInput i, ushort proj = 0x00ff);

        void WriteData<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>;
    }
}