using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// Represents A source for object persistence.
    ///
    public interface ISource
    {
        bool Get(string name, ref bool v);

        bool Get(string name, ref short v);

        bool Get(string name, ref int v);

        bool Get(string name, ref long v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref Number v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref NpgsqlPoint v);

        bool Get(string name, ref char[] v);

        bool Get(string name, ref string v);

        bool Get(string name, ref byte[] v);

        bool Get(string name, ref ArraySegment<byte>? v);

        bool Get<D>(string name, ref D v, byte z = 0) where D : IData, new();

        bool Get(string name, ref Obj v);

        bool Get(string name, ref Arr v);

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get<D>(string name, ref D[] v, byte z = 0) where D : IData, new();
    }
}