using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// Web client request/response content wrapper.
    ///
    public class WebCall : HttpContent, IDisposable
    {

        HttpRequestMessage request;

        IContent content;


        HttpResponseMessage response;

        public WebCall(IContent content)
        {
            this.content = content;
        }

        public void SetHeader(string name, string v)
        {
            request.Headers.Add(name, v);
        }

        public void Send(string url, IContent cont)
        {

        }

        public void GetJ(string url, Action<JsonContent> cont)
        {

        }

        public void GetXml(string url, Action<XmlContent> cont)
        {

        }

        public void PostJ(string url, Action<JsonContent> cont)
        {

        }

        public void PostXml(string url, Action<XmlContent> cont)
        {

        }

        public void PostForm(string url, Action<FormContent> cont)
        {

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


        //
        // RESPONSE
        //

        public Obj ReadJObj()
        {
            return null;
        }

        public Arr ReadJArr()
        {
            return null;
        }

        public P ReadObj<P>(byte z = 0) where P : IBean, new()
        {
            return default(P);
        }

        public P[] ReadArr<P>(byte z = 0) where P : IBean, new()
        {
            return null;
        }

        public string Header(string name)
        {
            // return response.Headers.TryGetValues
            return null;
        }

        public int StatusCode => (int)response.StatusCode;
    }

}