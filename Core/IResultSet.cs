using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IResultSet : ISource
    {
        bool Got(ref bool v);

        bool Got(ref short v);

        bool Got(ref int v);

        bool Got(ref long v);

        bool Got(ref decimal v);

        bool Got(ref DateTime v);

        bool Got(ref string v);

        bool Got<T>(ref T v) where T : IPersist, new();

        bool Got<T>(ref List<T> v) where T : IPersist, new();

        bool Got(ref byte[] v);

        bool Got(ref JObj v);

        bool Got(ref JArr v);

        //
        // OP

        bool NextRow();

        bool NextResult();

    }
}