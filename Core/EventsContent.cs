using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A binary-only content class that generates multipart/events content packing a number of events.
    ///
    public class EventsContent : HttpContent, IContent
    {
        byte[] bytebuf;

        int size;

        public byte[] ByteBuf => bytebuf;

        public char[] CharBuf => null;

        public bool IsPoolable => true;

        public bool IsBinary => true;

        public DateTime? Modified => null;

        public int Size => size;

        public string CType => "multipart/events";

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            throw new NotImplementedException();
        }

        protected override bool TryComputeLength(out long length)
        {
            throw new NotImplementedException();
        }
    }
}