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
            byte[] b = reqBytes.Array;
            if (b != null)
            {
                BufferPool.Return(b);
            }
        }


        public WebSub Controller { get; }

        public WebAction Action { get; }

        public string Var { get; internal set; }

        public IToken Token { get; }

        public bool IsSuspended { get; set; }


        //
        // REQUEST ACCESSORS
        //

        bool reqRcved;

        // request body bytes
        ArraySegment<byte> reqBytes;

        // request content that is parsed , can be IFormCollection, ISerial, etc.
        private object reqContent;

        /// <summary>
        /// Receiveds request body into a buffer held by the buffer field.
        ///</summary>
        internal async void TryReceiveAsync()
        {
            if (reqBytes.Array != null) return;

            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen.Value;
                // borrow a byte array from buffer pool
                byte[] buf = BufferPool.Borrow(len);
                int num = await req.Body.ReadAsync(buf, 0, len);
                reqBytes = new ArraySegment<byte>(buf, 0, num);
            }
        }

        public ArraySegment<byte> Bytes
        {
            get
            {
                TryReceiveAsync();
                return reqBytes;
            }
        }

        public ISerialReader Serial
        {
            get
            {
                if (reqContent == null)
                {
                    TryReceiveAsync();

                    if (reqBytes.Array != null)
                    {
                        string ctype = Request.ContentType;
                        if ("application/jsob".Equals(ctype))
                        {
                            reqContent = new JsobContent(reqBytes);
                        }
                        else
                        {
                            reqContent = new JsonContent(reqBytes);
                        }
                    }
                }
                return (ISerialReader)reqContent;
            }
        }

        public T GetSerial<T>() where T : ISerial, new()
        {
            ISerialReader r = this.Serial;
            T o = new T();
            r.Read(out o);
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

        public CachePolicy Policy { get; set; }

        public IContent Content { get; set; }

        public void SetContent<T>(T obj) where T : ISerial
        {
            SetSerialObj(obj, false);
        }

        public void SetSerialObj<T>(T obj) where T : ISerial
        {
            JsonContent cnt = new JsonContent(16 * 1024);
            cnt.Write(obj);

            Content = cnt;
        }

        public void SetSerialObj<T>(T obj, bool binary) where T : ISerial
        {
            DynamicContent cnt = binary ? new JsobContent(16 * 1024) : (DynamicContent)new JsonContent(16 * 1024);
            ((ISerialWriter)cnt).Write(obj);
            Content = cnt;
        }

        internal void WriteContent()
        {
            if (Content != null)
            {
                HttpResponse resp = Response;
                resp.ContentLength = Content.Length;
                resp.ContentType = Content.Type;
                resp.Body.Write(Content.Buffer, 0, Content.Length);
            }
        }

        internal Task WriteContentAsync()
        {
            if (Content != null)
            {
                HttpResponse rsp = Response;
                rsp.ContentLength = Content.Length;
                rsp.ContentType = Content.Type;

                // etag

                //
                return rsp.Body.WriteAsync(Content.Buffer, 0, Content.Length);
            }
            return Task.CompletedTask;
        }

    }
}