using System;

namespace ChainFx
{
    /// <summary>
    /// Represents an output sink that can be a dataset, a single data object, or only some of its data fields.
    /// </summary>
    public interface ISink
    {
        void PutNull(string name);

        void Put(string name, JNumber v);

        void Put(string name, bool v);

        void Put(string name, char v);

        void Put(string name, short v);

        void Put(string name, int v);

        void Put(string name, long v);

        void Put(string name, double v);

        void Put(string name, decimal v);

        void Put(string name, DateTime v);

        void Put(string name, string v);

        void Put(string name, ArraySegment<byte> v);

        void Put(string name, byte[] v);

        void Put(string name, short[] v);

        void Put(string name, int[] v);

        void Put(string name, long[] v);

        void Put(string name, string[] v);

        void Put(string name, JObj v); // essentially a map

        void Put(string name, JArr v);

        void Put(string name, XElem v);

        void Put(string name, IData v, short proj = 0xff);

        void Put<D>(string name, D[] v, short proj = 0xff) where D : IData;

        void PutFromSource(ISource s);
    }
}