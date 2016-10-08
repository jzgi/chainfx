using System;

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

        bool Got(ref char[] v);

        bool Got(ref string v);

        bool Got(ref byte[] v);

        bool Got<T>(ref T v, int x = -1) where T : IPersist, new();

        bool Got(ref JObj v);

        bool Got(ref JArr v);

        bool Got(ref short[] v);

        bool Got(ref int[] v);

        bool Got(ref long[] v);

        bool Got(ref string[] v);

        bool Got<T>(ref T[] v, int x = -1) where T : IPersist, new();

        //
        // OP

        bool NextRow();

        bool NextResult();

    }
}