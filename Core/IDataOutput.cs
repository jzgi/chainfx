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

        R PutRaw(string name, string raw);

        void Group(string label);

        void UnGroup();

        R Put(string name, bool value, string label = null, Func<bool, string> opt = null);

        R Put(string name, short value, string label = null, IOptable<short> opt = null);

        R Put(string name, int value, string label = null, IOptable<int> opt = null);

        R Put(string name, long value, string label = null, IOptable<long> opt = null);

        R Put(string name, double value, string label = null);

        R Put(string name, decimal value, string label = null, char format = '\0');

        R Put(string name, DateTime value, string label = null);

        R Put(string name, string value, string label = null, IOptable<string> opt = null);

        R Put(string name, ArraySegment<byte> value, string label = null);

        R Put(string name, short[] value, string label = null, IOptable<short> opt = null);

        R Put(string name, int[] value, string label = null, IOptable<int> opt = null);

        R Put(string name, long[] value, string label = null, IOptable<long> opt = null);

        R Put(string name, string[] value, string label = null, IOptable<string> opt = null);

        R Put(string name, Dictionary<string, string> value, string label = null);

        R Put(string name, IData value, int proj = 0x00ff, string label = null);

        R Put<D>(string name, D[] value, int proj = 0x00ff, string label = null) where D : IData;
    }
}