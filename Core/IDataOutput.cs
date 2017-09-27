using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a sink for data output.
    /// </summary>
    public interface IDataOutput<out R> where R : IDataOutput<R>
    {
        R PutNull(string name);

        R Put(string name, JNumber value);

        R Put(string name, IDataInput value);

        R Put(string name, bool value);

        R Put(string name, short value);

        R Put(string name, int value);

        R Put(string name, long value);

        R Put(string name, double value);

        R Put(string name, decimal value);

        R Put(string name, DateTime value);

        R Put(string name, string value);

        R Put(string name, ArraySegment<byte> value);

        R Put(string name, short[] value);

        R Put(string name, int[] value);

        R Put(string name, long[] value);

        R Put(string name, string[] value);

        R Put(string name, Dictionary<string, string> value);

        R Put(string name, IData value, int proj = 0x00ff);

        R Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData;
    }
}