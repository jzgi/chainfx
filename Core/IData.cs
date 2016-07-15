namespace Greatbone.Core
{
    ///
    /// Rerepsents a data object that can be converted from/to various data inputs/outputs.
    ///
    public interface IData
    {
        void From(IDataInput i);

        void To(IDataOutput o);
    }
}