using System;

namespace Greatbone.Core
{
    ///
    /// Represents an destination for object persistence.
    ///
    public interface ISink<R> where R : ISink<R>
    {
        R PutNull(string name);

        R Put(string name, bool v);

        R Put(string name, short v);

        R Put(string name, int v);

        R Put(string name, long v);

        R Put(string name, decimal v);

        R Put(string name, Number v);

        R Put(string name, DateTime v);

        R Put(string name, char[] v);

        R Put(string name, string v, int maxlen = 0);

        R Put(string name, byte[] v);

        R Put(string name, ArraySegment<byte> v);

        R Put<D>(string name, D v, byte z = 0) where D : IDat;

        R Put(string name, Obj v);

        R Put(string name, Arr v);

        R Put(string name, short[] v);

        R Put(string name, int[] v);

        R Put(string name, long[] v);

        R Put(string name, string[] v);

        R Put<D>(string name, D[] v, byte z = 0) where D : IDat;
    }
}