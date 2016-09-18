using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    ///
    /// The encapsulation of a web request/response exchange context.
    ///
    /// buffer pooling -- reduces GC overhead when dealing with asynchronous request/response I/O
    ///
    public class WebContext : DefaultHttpContext, IDisposable
    {
        internal WebContext(IFeatureCollection features) : base(features)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public WebSub Controller { get; }

        public WebAction Action { get; }

        public string Var { get; internal set; }

        public IToken Token { get; }

        public bool IsSuspended { get; set; }


        //
        // REQUEST ACCESSORS
        //

        // in raw bytes
        private byte[] buffer;

        private int count;

        // the content is parsed on demand of the application
        private object content;


        /// <summary>
        /// Receiveds request body into a buffer held by the buffer field.
        ///</summary>
        internal async void ReceiveAsync()
        {
            long? clen = Request.ContentLength;
            if (buffer == null && clen > 0)
            {
                int len = (int)clen.Value;
                // borrow a byte array from buffer pool
                buffer = BufferPool.Lease(len);
                count = await Request.Body.ReadAsync(buffer, 0, len);
            }
        }

        public ArraySegment<byte> Bytes
        {
            get
            {
                if (buffer == null)
                {
                    ReceiveAsync();
                }
                return new ArraySegment<byte>(buffer, 0, count);
            }
        }

        public ISerialReader Reader
        {
            get
            {
                if (content == null)
                {
                    if (buffer == null)
                    {
                        ReceiveAsync();
                    }
                    // init content
                    string ctype = Request.ContentType;
                    long? clen = Request.ContentLength;
                    if ("application/bjson".Equals(ctype))
                    {
                        return new JsobContent(buffer, (int)clen);
                    }
                    else
                    {
                        return new JsonContent(buffer, (int)clen);
                    }
                }
                return null;
            }
        }

        public T Serial<T>() where T : ISerial, new()
        {
            ISerialReader r = this.Reader;
            T o = new T();
            r.Read(ref o);
            return o;
        }

        public bool GetParam(string name, out int value)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string v = values[0];
                int i;
                if (Int32.TryParse(v, out i))
                {
                    value = i;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        public bool GetParam(string name, out string value)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                value = values[0];
                return true;
            }
            value = null;
            return false;
        }


        public bool GetField(string name, out int value)
        {
            StringValues values;
            if (Request.Form.TryGetValue(name, out values))
            {
                string v = values[0];
                int i;
                if (Int32.TryParse(v, out i))
                {
                    value = i;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        public bool GetField(string name, out string value)
        {
            StringValues values;
            if (Request.Form.TryGetValue(name, out values))
            {
                value = values[0];
                return true;
            }
            value = null;
            return false;
        }


        //
        // RESPONSE ACCESSORS
        //

        public int StatusCode { get { return Response.StatusCode; } set { Response.StatusCode = value; } }

        public CachePolicy CachePolicy { get; set; }

        public IContent Content { get; set; }

        public void SetContent<T>(T obj) where T : ISerial
        {
            SetContent(obj, false);
        }

        public void SetContentAsJson<T>(T obj) where T : ISerial
        {
            JsonContent cnt = new JsonContent(16 * 1024);
            cnt.Write(obj);

            Content = cnt;
        }

        public void SetContent<T>(T obj, bool binary) where T : ISerial
        {
            DynamicContent cnt = binary ? new JsobContent(16 * 1024) : (DynamicContent)new JsonContent(16 * 1024);
            ((ISerialWriter)cnt).Write(obj);
            Content = cnt;
        }

        internal void WriteContent()
        {
            if (Content != null)
            {
                Response.ContentLength = Content.Count;
                Response.ContentType = Content.Type;
                Response.Body.Write(Content.Buffer, 0, Content.Count);
            }
        }

        internal Task WriteContentAsync()
        {
            if (Content != null)
            {
                Response.ContentLength = Content.Count;
                Response.ContentType = Content.Type;

                // etag


                //
                return Response.Body.WriteAsync(Content.Buffer, 0, Content.Count);
            }
            return Task.CompletedTask;
        }

    }
}