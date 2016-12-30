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

        bool Get(string name, ref double v);

        bool Get(string name, ref decimal v);

        bool Get(string name, ref JNumber v);

        bool Get(string name, ref DateTime v);

        bool Get(string name, ref NpgsqlPoint v);

        bool Get(string name, ref char[] v);

        bool Get(string name, ref string v);

        bool Get(string name, ref byte[] v);

        bool Get(string name, ref ArraySegment<byte>? v);

        bool Get<D>(string name, ref D v, byte bits = 0) where D : IData, new();

        bool Get(string name, ref JObj v);

        bool Get(string name, ref JArr v);

        bool Get(string name, ref short[] v);

        bool Get(string name, ref int[] v);

        bool Get(string name, ref long[] v);

        bool Get(string name, ref string[] v);

        bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IData, new();
    }
}