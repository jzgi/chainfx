namespace Greatbone.Core
{
    /// <summary>
    /// A data record that follows certain input/ouput paradigm.
    /// </summary>
    public interface IData
    {
        void Read(IDataInput i, byte proj = 0x1f);

        void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>;
    }
}