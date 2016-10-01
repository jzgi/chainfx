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

        public WebSub Controller { get; }

        public WebAction Action { get; }

        public string Var { get; internal set; }

        public IToken Token { get; }


        //
        // REQUEST ACCESSORS
        //

        bool reqRcved;

        // request body bytes
        ArraySegment<byte> reqBytes;

        // request content that is parsed
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

        public object Doc
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
                            JsonParser parser = new JsonParser(reqBytes.Array);
                            reqContent = parser.Parse();
                        }
                    }
                }
                return reqContent;
            }
        }

        public T Dat<T>() where T : IPersist, new()
        {
            Obj obj = (Obj)Doc;
            T dat = new T();
            dat.Load(obj, 0);
            return dat;
        }

        public bool Get(string name, ref int value)
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
            return false;
        }

        public bool Get(string name, ref string value)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                value = values[0];
                return true;
            }
            return false;
        }


        //
        // RESPONSE ACCESSORS
        //

        public int StatusCode { get { return Response.StatusCode; } set { Response.StatusCode = value; } }

        public CachePolicy Caching { get; set; }

        public IContent Content { get; set; }

        public void Respond<T>(T obj) where T : IPersist
        {
            JsonContent json = new JsonContent(4 * 1024);
            obj.Save(json, 0);
            Content = json;
        }

        public void Respond(int status, Action<JsonContent> a)
        {
            JsonContent json = new JsonContent(16 * 1024);
            a?.Invoke(json);
            Content = json;
            StatusCode = status;
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


        public void Dispose()
        {
            // return request content buffer
            byte[] b = reqBytes.Array;
            if (b != null)
            {
                BufferPool.Return(b);
            }

            // return response content buffer
            if (Content is DynamicContent)
            {
                BufferPool.Return(Content.Buffer);
            }
        }

    }
}