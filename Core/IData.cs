namespace Greatbone.Core
{
    ///
    /// A data record that follows certain input/ouput paradigm.
    ///
    public interface IData
    {
        void Read(IDataInput i, int proj = 0x00ff);

        void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>;
    }
}