using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// A data source or result set consisting of many source objects.
    ///
    public interface IDataSetInput : IDataInput
    {
        bool Next();

        D[] ToArray<D>(byte flags = 0) where D : IData, new();

        List<D> ToList<D>(byte flags = 0) where D : IData, new();
    }
}