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

        // received body bytes
        ArraySegment<byte> bytesSeg;

        // parsed request entity, can be Doc or Form
        object entity;

        async void ReceiveAsync()
        {
            if (bytesSeg != null)
            {
                return;
            }
            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen.Value;
                byte[] buffer = BufferPool.Borrow(len);
                int count = await req.Body.ReadAsync(buffer, 0, len);
                bytesSeg = new ArraySegment<byte>(buffer, 0, count);
            }
        }

        public ArraySegment<byte> BytesSeg
        {
            get
            {
                if (bytesSeg.Array == null) { ReceiveAsync(); }
                return bytesSeg;
            }
        }

        void ParseEntity()
        {
            if (entity == null)
            {
                ArraySegment<byte> bseg = BytesSeg;
                if (bseg.Array != null)
                {
                    string ctyp = Request.ContentType;
                    if ("application/x-www-form-urlencoded".Equals(ctyp))
                    {
                        FormParse parse = new FormParse(bseg);
                        entity = parse.Parse();
                    }
                    else
                    {
                        JParse parse = new JParse(bseg);
                        entity = parse.Parse();
                    }
                }
            }
        }

        public Form Form
        {
            get
            {
                ParseEntity();
                return entity as Form;
            }
        }

        public JObj JObj
        {
            get
            {
                ParseEntity();
                return entity as JObj;
            }
        }

        public JArr JArr
        {
            get
            {
                ParseEntity();
                return entity as JArr;
            }
        }

        public T Obj<T>(int x = -1) where T : IPersist, new()
        {
            JObj jo = JObj;
            T obj = new T();
            obj.Load(jo);
            return obj;
        }

        public T[] Arr<T>(int x = -1) where T : IPersist, new()
        {
            return null;
        }

        public bool Got(string name, ref int value)
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

        public bool Got(string name, ref string value)
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
        // RESPONSE
        //

        public int StatusCode
        {
            get { return Response.StatusCode; }
            set { Response.StatusCode = value; }
        }

        public IContent Content { get; set; }

        internal bool? Pub { get; set; }

        internal int MaxAge { get; set; }

        public void Respond<T>(int status, T obj, ushort x = 0xffff, bool? pub = false, int maxage = 0) where T : IPersist
        {
            Respond(status, jcont => jcont.PutObj(obj, x), pub, maxage);
        }

        public void Respond<T>(int status, T[] arr, ushort x = 0xffff, bool? pub = false, int maxage = 0) where T : IPersist
        {
            Respond(status, jcont => jcont.PutArr(arr, x), pub, maxage);
        }

        public void Respond(int status, Action<JContent> a, bool? pub = false, int maxage = 0)
        {
            JContent jcont = new JContent(8 * 1024);
            a?.Invoke(jcont);

            Respond(status, jcont, pub, maxage);
        }

        public void Respond(int status, Action<HtmlContent> a, bool? pub = true, int maxage = 1000)
        {
            StatusCode = status;

            this.Pub = pub;
            this.MaxAge = maxage;

            HtmlContent html = new HtmlContent(16 * 1024);
            a?.Invoke(html);
            Content = html;
        }

        public void Respond(int status, IContent cont, bool? pub = null, int maxage = 0)
        {
            StatusCode = status;
            if (cont != null)
            {
                Content = cont;
                Pub = pub;
                MaxAge = maxage;
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


        public void Dispose()
        {
            // return request content buffer
            byte[] buf = bytesSeg.Array;
            if (buf != null)
            {
                BufferPool.Return(buf);
            }

            // return response content buffer
            if (Content is DynamicContent)
            {
                BufferPool.Return(Content.Buffer);
            }
        }

    }
}