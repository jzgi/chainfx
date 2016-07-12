namespace Greatbone.Core
{
    ///
    /// The object can be turned from various source contents and to various target contents.
    public interface IData
    {
        void From(IDataInput i);

        void To(IDataOutput o);
    }
}