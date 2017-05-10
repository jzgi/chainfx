using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// Represents a sink for data output.
    ///
    public interface IDataOutput<out R> where R : IDataOutput<R>
    {
        R PutNull(string name);

        R Put(string name, JNumber v);

        R Put(string name, IDataInput v);

        R PutRaw(string name, string raw);

        void Group(string label);

        void UnGroup();

        R Put(string name, bool v, Func<bool, string> opt = null, string label = null);

        R Put(string name, short v, Opt<short> opt = null, string label = null);

        R Put(string name, int v, Opt<int> opt = null, string label = null);

        R Put(string name, long v, Opt<long> opt = null, string label = null);

        R Put(string name, double v, string label = null);

        R Put(string name, decimal v, string label = null, char format = '\0');

        R Put(string name, DateTime v, string label = null);

        R Put(string name, string v, Opt<string> opt = null, string label = null);

        R Put(string name, ArraySegment<byte> v, string label = null);

        R Put(string name, short[] v, Opt<short> opt = null, string label = null);

        R Put(string name, int[] v, Opt<int> opt = null, string label = null);

        R Put(string name, long[] v, Opt<long> opt = null, string label = null);

        R Put(string name, string[] v, Opt<string> opt = null, string label = null);

        R Put(string name, Dictionary<string, string> v, string label = null);

        R Put(string name, IData v, short proj = 0, string label = null);

        R Put<D>(string name, D[] v, short proj = 0, string label = null) where D : IData;
    }
}