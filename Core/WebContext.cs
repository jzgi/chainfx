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
        object data;

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

        /// <summary>
        /// A data object model of type Obj, Arr ,Elem or Form, constructed from parsing the JSON or XML entity.  
        /// </summary>
        /// <returns></returns>
        public object Data
        {
            get
            {
                if (data == null)
                {
                    TryReceiveAsync();

                    if (buffer != null)
                    {
                        string ctype = Request.ContentType;
                        if ("application/jsob".Equals(ctype))
                        {
                            JParse parse = new JParse(buffer, count);
                            data = parse.Parse();
                        }
                        else if ("application/x-www-form-urlencoded".Equals(ctype))
                        {
                            FormParse parse = new FormParse(buffer, count);
                            data = parse.Parse();
                        }
                    }
                }
                return data;
            }
        }

        public T Get<T>(int x = -1) where T : IPersist, new()
        {
            JObj mo = (JObj)Data;
            T obj = new T();
            obj.Load(mo);
            return obj;
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

        public int StatusCode { get { return Response.StatusCode; } set { Response.StatusCode = value; } }

        internal bool? pub;

        internal int maxage;

        public IContent Content { get; set; }

        public void SetObj<T>(int status, T obj, int x = -1, bool? pub = true, int maxage = 1000) where T : IPersist
        {
            SetJson(status, jcont => jcont.PutObj(obj, x), pub, maxage);
        }

        public void SetArr<T>(int status, T[] arr, int x = -1, bool? pub = true, int maxage = 1000) where T : IPersist
        {
            SetJson(status, jcont => jcont.PutArr(arr, x), pub, maxage);
        }

        public void SetJson(int status, Action<JContent> a, bool? pub = true, int maxage = 1000)
        {
            StatusCode = status;

            this.pub = pub;
            this.maxage = maxage;

            JContent json = new JContent(8 * 1024);
            a?.Invoke(json);
            Content = json;
        }

        public void SetHtml(int status, Action<HtmlContent> a, bool? pub = true, int maxage = 1000)
        {
            StatusCode = status;

            this.pub = pub;
            this.maxage = maxage;

            HtmlContent html = new HtmlContent(16 * 1024);
            a?.Invoke(html);
            Content = html;
        }

        public void SendBytes(int status, byte[] v, bool? pub = true, int maxage = 1000)
        {
            StatusCode = status;

            this.pub = pub;
            this.maxage = maxage;

            HtmlContent html = new HtmlContent(16 * 1024);
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