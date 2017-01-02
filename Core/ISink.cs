using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// Represents an destination for object persistence.
    ///
    public interface ISink<out R> where R : ISink<R>
    {
        R PutNull(string name);

        R Put(string name, bool v);

        R Put(string name, short v);

        R Put(string name, int v);

        R Put(string name, long v);

        R Put(string name, double v);

        R Put(string name, decimal v);

        R Put(string name, JNumber v);

        R Put(string name, DateTime v);

        R Put(string name, NpgsqlPoint v);

        R Put(string name, char[] v);

        R Put(string name, string v, bool? anylen = null);

        R Put(string name, byte[] v);

        R Put(string name, ArraySegment<byte> v);

        R Put<D>(string name, D v, byte bits = 0) where D : IDat;

        R Put(string name, JObj v);

        R Put(string name, JArr v);

        R Put(string name, short[] v);

        R Put(string name, int[] v);

        R Put(string name, long[] v);

        R Put(string name, string[] v);

        R Put<D>(string name, D[] v, byte bits = 0) where D : IDat;
    }
}