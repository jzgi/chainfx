using System;

namespace Greatbone
{
    /// <summary>
    /// The pack of a set of recent events.
    /// </summary>
    public class FlowContext : ISource
    {
        byte[] buffer;

        int length;

        int pos;

        public FlowContext(byte[] buffer, int length)
        {
            this.buffer = buffer;
            this.length = length;
        }

        public long RevId => pos;

        public bool Get(string name, ref bool v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref double v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }


        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public ISource Let(out bool v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out short v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out int v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out long v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out double v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out decimal v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out DateTime v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out string v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out int[] v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out long[] v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out string[] v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out JObj v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out JArr v)
        {
            throw new NotImplementedException();
        }

        public ISource Let<D>(out D v, byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public ISource Let<D>(out D[] v, byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool DataSet => true;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            throw new NotImplementedException();
        }
    }
}