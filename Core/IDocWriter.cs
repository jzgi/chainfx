using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// DataReader/ParameterCollection, JSON or binary
    public interface IDocWriter
    {
        void WriteStart();

        void WriteEnd();

        void Write(string name, int value);

        void Write(string name, decimal value);

        void Write(string name, string value);

        void Write<T>(string name, List<T> value) where T : IDoc;
    }
}