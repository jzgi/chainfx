using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents an output sink for a dataset, a single data object, or only some of its data fields.
    /// </summary>
    public interface ISink<out R> where R : ISink<R>
    {
        // put dataset opening, if any
        R PutOpen();

        // put dataset closing, if any
        R PutClose();

        // put data object start, if any
        R PutStart();

        // put data object end, if any
        R PutEnd();

        R PutNull(string name);

        R Put(string name, JNumber v);

        R Put(string name, ISource v);

        R Put(string name, bool v);

        R Put(string name, short v);

        R Put(string name, int v);

        R Put(string name, long v);

        R Put(string name, double v);

        R Put(string name, decimal v);

        R Put(string name, DateTime v);

        R Put(string name, string v);

        R Put(string name, ArraySegment<byte> v);

        R Put(string name, short[] v);

        R Put(string name, int[] v);

        R Put(string name, long[] v);

        R Put(string name, string[] v);

        R Put(string name, Map<string, string> v);

        R Put(string name, IData v, byte proj = 0x0f);

        R Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData;
    }
}