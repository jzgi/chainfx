using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents an output sink fovoid a dataset, a single data object, ovoid only some of its data fields.
    /// </summary>
    public interface ISink
    {
        // put dataset opening, if any
        void PutOpen();

        // put dataset closing, if any
        void PutClose();

        // put data object start, if any
        void PutStart();

        // put data object end, if any
        void PutEnd();

        void PutNull(string name);

        void Put(string name, JNumber v);

        void Put(string name, ISource v);

        void Put(string name, bool v);

        void Put(string name, short v);

        void Put(string name, int v);

        void Put(string name, long v);

        void Put(string name, double v);

        void Put(string name, decimal v);

        void Put(string name, DateTime v);

        void Put(string name, string v);

        void Put(string name, ArraySegment<byte> v);

        void Put(string name, short[] v);

        void Put(string name, int[] v);

        void Put(string name, long[] v);

        void Put(string name, string[] v);

        void Put(string name, Map<string, string> v);

        void Put(string name, IData v, byte proj = 0x0f);

        void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData;
    }
}