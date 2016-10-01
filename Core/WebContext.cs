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
        // REQUEST
        //

        byte[] buffer;

        int count;

        // parsed request entity, can be Doc or Form
        object entity;

        /// <summary>
        /// Receiveds request body into a buffer held by the buffer field.
        ///</summary>
        internal async void TryReceiveAsync()
        {
            if (buffer != null) return;

            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen.Value;
                buffer = BufferPool.Borrow(len);
                count = await req.Body.ReadAsync(buffer, 0, len);
            }
        }

        public ArraySegment<byte> Bytes
        {
            get
            {
                TryReceiveAsync();
                return new ArraySegment<byte>(buffer, 0, count);
            }
        }

        public object Doc
        {
            get
            {
                if (entity == null)
                {
                    TryReceiveAsync();

                    if (buffer != null)
                    {
                        string ctype = Request.ContentType;
                        if ("application/jsob".Equals(ctype))
                        {
                            JsonParser parser = new JsonParser(buffer, count);
                            entity = parser.Parse();
                        }
                    }
                }
                return entity;
            }
        }

        public T Dat<T>(int x) where T : IPersist, new()
        {
            Obj obj = (Obj)Doc;
            T dat = new T();
            dat.Load(obj, x);
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

        public void SetDat<T>(T obj) where T : IPersist
        {
            JsonContent json = new JsonContent(4 * 1024);
            obj.Save(json, 0);
            Content = json;
        }

        public void SetJson(int status, Action<JsonContent> a)
        {
            StatusCode = status;
            JsonContent json = new JsonContent(8 * 1024);
            a?.Invoke(json);
            Content = json;
        }


        public void SetHtml(int status, Action<HtmlContent> a)
        {
            StatusCode = status;
            HtmlContent html = new HtmlContent(16 * 1024);
            a?.Invoke(html);
            Content = html;
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
            if (buffer != null)
            {
                BufferPool.Return(buffer);
            }

            // return response content buffer
            if (Content is DynamicContent)
            {
                BufferPool.Return(Content.Buffer);
            }
        }

    }
}