using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// DataReader/ParameterCollection, JSON or binary
    public interface ITarget
    {
        void Put(string name, int value);

        void Put(string name, decimal value);

        void Put(string name, string value);

        void Put<T>(string name, List<T> value) where T : IPersistable;
    }
}