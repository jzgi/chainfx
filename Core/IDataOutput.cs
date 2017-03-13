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

        R Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false);

        R Put(string name, short v, Map<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, int v, Map<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, long v, Map<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, string v, Map<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false);

        R Put(string name, ArraySegment<byte> v, string Label = null, string Size = null, string Ratio = null, bool Required = false);

        R Put(string name, short[] v, Map<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put(string name, int[] v, Map<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put(string name, long[] v, Map<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put(string name, string[] v, Map<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put(string name, Map v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put(string name, IData v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false);

        R Put<D>(string name, D[] v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData;

        R Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData;
    }
}