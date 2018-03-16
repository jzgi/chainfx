using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone
{
    public class FlowContent : HttpContent, IContent, ISink
    {
        // NOTE: HttpResponseStream doesn't have internal buffer
        byte[] bytebuf;

        // number of bytes or chars
        int count;

        public FlowContent(int capacity)
        {
            bytebuf = BufferUtility.GetByteBuffer(capacity);
            count = 0;
        }

        public string Type => "application/x-flow";

        public byte[] ByteBuffer => bytebuf;

        public char[] CharBuffer => null;

        public string ETag => null;

        public int Size => count;

        //
        // SINK
        //

        public void PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, JNumber v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, ISource v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, double v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, IData v, byte proj = 15)
        {
            throw new NotImplementedException();
        }

        public void Put<D>(string name, D[] v, byte proj = 15) where D : IData
        {
            throw new NotImplementedException();
        }

        public void PutFrom(ISource s)
        {
            throw new NotImplementedException();
        }

        //
        // CLIENT CONTENT
        //
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return stream.WriteAsync(bytebuf, 0, count);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = count;
            return true;
        }

        public ArraySegment<byte> ToByteAs()
        {
            return new ArraySegment<byte>(bytebuf, 0, count);
        }
    }
}