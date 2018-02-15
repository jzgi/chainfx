using System;

namespace Greatbone.Core
{
    public class FlowContent : DynamicContent, IDataOutput<FlowContent>
    {
        public FlowContent(int capacity) : base(true, capacity)
        {
        }

        public override string Type { get; }

        public FlowContent PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, JNumber v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, IDataInput v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, double v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put(string name, IData v, byte proj = 15)
        {
            throw new NotImplementedException();
        }

        public FlowContent Put<D>(string name, D[] v, byte proj = 15) where D : IData
        {
            throw new NotImplementedException();
        }
    }
}