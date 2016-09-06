using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Greatbone.Core
{
    ///
    /// The wrapper of a HTTP response, providing efficient output methods and cache control.
    ///
    public class WebResponse : DefaultHttpResponse
    {
        public WebResponse(HttpContext ctx) : base(ctx)
        {
        }

        public CachePolicy CachePolicy { get; set; }

        public IContent Content { get; set; }

//		public void SetJson(object obj)
//		{
//			string text = JsonConvert.SerializeObject(obj);
//
//			byte[] bytes = Encoding.UTF8.GetBytes(text);
//			Content = new DummyContent
//			{
//				Type = "application/json",
//				Buffer = bytes,
//				Offset = 0,
//				Count = bytes.Length
//			};
//		}

        public void SetContent<T>(T obj) where T : ISerial
        {
            SetContent(obj, false);
        }

        public void SetContentAsJson<T>(T obj) where T : ISerial
        {
            byte[] buf = BufferPool.Lease(16 * 1024);
            JsonContent cnt = new JsonContent(buf);
            cnt.Write(obj);

            Content = cnt;
        }

        public void SetContent<T>(T obj, bool binary) where T : ISerial
        {
            byte[] buf = BufferPool.Lease(16 * 1024);
            DynamicContent cnt = binary ? new BJsonContent(buf) : (DynamicContent) new JsonContent(buf);
            ((ISerialWriter) cnt).Write(obj);
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