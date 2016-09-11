using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Greatbone.Core
{
    ///
    /// <summary>The wrapper of a HTTP response that provides efficient output methods and cache control.</summary>
    ///
    public class WebResponse : DefaultHttpResponse
    {
        public WebResponse(HttpContext ctx) : base(ctx)
        {
        }

        public CachePolicy CachePolicy { get; set; }

        public IContent Content { get; set; }

        public void SetContent<T>(T obj) where T : ISerial
        {
            SetContent(obj, false);
        }

        public void SetContentAsJson<T>(T obj) where T : ISerial
        {
            JsonWriter cnt = new JsonWriter(16 * 1024);
            cnt.Write(obj);

            Content = cnt;
        }

        public void SetContent<T>(T obj, bool binary) where T : ISerial
        {
            ContentWriter cnt = binary ? new JsonbWriter(16 * 1024) : (ContentWriter)new JsonWriter(16 * 1024);
            ((ISerialWriter)cnt).Write(obj);
            Content = cnt;
        }

        internal void WriteContent()
        {
            if (Content != null)
            {
                ContentLength = Content.Count;
                ContentType = Content.Type;
                Body.Write(Content.Buffer, 0, Content.Count);
            }
        }

        internal Task WriteContentAsync()
        {
            if (Content != null)
            {
                ContentLength = Content.Count;
                ContentType = Content.Type;

                // etag


                //
                return Body.WriteAsync(Content.Buffer, 0, Content.Count);
            }
            return Task.CompletedTask;
        }
    }
}