using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents an output sink..
    /// </summary>
    public interface IDataOutput<out R> where R : IDataOutput<R>
    {
        R PutNull(string name);

        R Put(string name, JNumber v);

        R Put(string name, IDataInput v);

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