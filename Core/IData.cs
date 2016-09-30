namespace Greatbone.Core
{
    /// <summary>
    /// Represents a data object that is compliant to standard data exchange mechanisms.
    /// </summary>
    public interface IData
    {
        void In(IDataIn i);

        void Out<R>(IDataOut<R> o) where R : IDataOut<R>;
    }
}