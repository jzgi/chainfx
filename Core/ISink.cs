using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents an destination for object persistence.
    /// </summary>
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

        R Put(string name, string v);

        R Put(string name, byte[] v);

        R Put(string name, ArraySegment<byte> v);

        R Put<V>(string name, V v, ushort x = 0) where V : IPersist;

        R Put(string name, JObj v);

        R Put(string name, JArr v);

        R Put(string name, short[] v);

        R Put(string name, int[] v);

        R Put(string name, long[] v);

        R Put(string name, string[] v);

        R Put<V>(string name, V[] v, ushort x = 0) where V : IPersist;

    }

}