using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// Web client request/response content wrapper.
    ///
    public class WebCall : HttpContent
    {

        HttpRequestMessage request;

        HttpResponseMessage response;

        IContent content;

        public WebCall(IContent content)
        {
            this.content = content;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return stream.WriteAsync(content.ByteBuffer, 0, content.Size);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Size;
            return true;
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            return Task.FromResult<Stream>((Stream)content);
        }
    }

}