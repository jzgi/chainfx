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

        R Put(string name, bool v, Func<bool, string> opt = null, string label = null, bool required = false);

        R Put(string name, short v, Opt<short> opt = null, string label = null, string help = null, short max = 0, short min = 0, short step = 0, bool @readonly = false, bool required = false);

        R Put(string name, int v, Opt<int> opt = null, string label = null, string help = null, int max = 0, int min = 0, int step = 0, bool @readonly = false, bool required = false);

        R Put(string name, long v, Opt<long> opt = null, string label = null, string help = null, long max = 0, long min = 0, long step = 0, bool @readonly = false, bool required = false);

        R Put(string name, double v, string label = null, string help = null, double max = 0, double min = 0, double Step = 0, bool @readonly = false, bool required = false);

        R Put(string name, decimal v, string label = null, string help = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false);

        R Put(string name, DateTime v, string label = null, DateTime max = default(DateTime), DateTime min = default(DateTime), int step = 0, bool @readonly = false, bool required = false);

        R Put(string name, string v, Opt<string> opt = null, string label = null, string help = null, string pattern = null, short max = 0, short min = 0, bool @readonly = false, bool required = false);

        R Put(string name, ArraySegment<byte> v, string label = null, string size = null, string ratio = null, bool required = false);

        R Put(string name, short[] v, Opt<short> opt = null, string label = null, string help = null, bool @readonly = false, bool Required = false);

        R Put(string name, int[] v, Opt<int> opt = null, string label = null, string help = null, bool @readonly = false, bool required = false);

        R Put(string name, long[] v, Opt<long> opt = null, string label = null, string help = null, bool @readonly = false, bool required = false);

        R Put(string name, string[] v, Opt<string> opt = null, string label = null, string help = null, bool @readonly = false, bool required = false);

        R Put(string name, Dictionary<string, string> v, string label = null, string help = null, bool @readonly = false, bool required = false);

        R Put(string name, IData v, int proj = 0, string label = null, string help = null, bool @readonly = false, bool required = false);

        R Put<D>(string name, D[] v, int proj = 0, string label = null, string help = null, bool @readonly = false, bool required = false) where D : IData;

        R Put<D>(string name, List<D> v, int proj = 0, string label = null, string help = null, bool @readonly = false, bool required = false) where D : IData;
    }
}