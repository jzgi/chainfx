using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    /// <summary>
    /// The encapsulation of a web request/response exchange context.
    /// </summary>
    public class WebContext : DefaultHttpContext, ISource, IDisposable
    {

        internal WebContext(IFeatureCollection features) : base(features)
        {
        }

        public WebSub Control { get; internal set; }

        public WebAction Action { get; internal set; }

        public IToken Token { get; internal set; }


        //
        // REQUEST
        //

        public bool IsGet => "GET".Equals(Request.Method);

        public bool IsPost => "POST".Equals(Request.Method);

        // received body bytes
        ArraySegment<byte>? bytesSeg;

        // parsed request entity, can be Doc or Form
        object entity;

        async void ReceiveAsync()
        {
            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen;
                byte[] buffer = BufferPool.Borrow(len);
                int count = await req.Body.ReadAsync(buffer, 0, len);
                bytesSeg = new ArraySegment<byte>(buffer, 0, count);
            }
        }

        void ParseEntity()
        {
            if (entity != null) return;

            if (bytesSeg == null) ReceiveAsync();

            if (bytesSeg != null)
            {
                string ctyp = Request.ContentType;
                if ("application/x-www-form-urlencoded".Equals(ctyp))
                {
                    FormParse parse = new FormParse(bytesSeg.Value);
                    entity = parse.Parse();
                }
                else
                {
                    JParse parse = new JParse(bytesSeg.Value);
                    entity = parse.Parse();
                }
            }
        }

        public ArraySegment<byte>? BytesSeg
        {
            get
            {
                if (bytesSeg == null) ReceiveAsync();
                return bytesSeg.Value;
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

        //
        // SOURCE FOR QUERY STRING
        //

        public bool Got(string name, ref bool v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                if ("true".Equals(str) || "1".Equals(str))
                {
                    v = true;
                    return true;
                }
                if ("false".Equals(str) || "0".Equals(str))
                {
                    v = false;
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref char v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                if (!string.IsNullOrEmpty(str))
                {
                    v = str[0];
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref short v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                short num;
                if (short.TryParse(str, out num))
                {
                    v = num;
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref int v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                int num;
                if (int.TryParse(str, out num))
                {
                    v = num;
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref long v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                long num;
                if (long.TryParse(str, out num))
                {
                    v = num;
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref decimal v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                decimal num;
                if (decimal.TryParse(str, out num))
                {
                    v = num;
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, ref Number v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref string v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                v = values[0];
                return true;
            }
            return false;
        }

        public bool Got(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Got<V>(string name, ref V v, uint x = 0) where V : IPersist, new()
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse parse = new JTextParse(str);
                JObj jo = (JObj)parse.Parse();
                v = new V();
                v.Load(jo, x);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JObj v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse parse = new JTextParse(str);
                v = (JObj)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JArr v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse parse = new JTextParse(str);
                v = (JArr)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(string name, ref short[] v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                int len = values.Count;
                v = new short[len];
                for (int i = 0; i < len; i++)
                {
                    string str = values[i];
                    short e = 0;
                    if (short.TryParse(str, out e))
                    {
                        v[i] = e;
                    }
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref int[] v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                int len = values.Count;
                v = new int[len];
                for (int i = 0; i < len; i++)
                {
                    string str = values[i];
                    int e = 0;
                    if (int.TryParse(str, out e))
                    {
                        v[i] = e;
                    }
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref long[] v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                int len = values.Count;
                v = new long[len];
                for (int i = 0; i < len; i++)
                {
                    string str = values[i];
                    long e = 0;
                    if (long.TryParse(str, out e))
                    {
                        v[i] = e;
                    }
                }
                return true;
            }
            return false;
        }

        public bool Got(string name, ref string[] v)
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                int len = values.Count;
                v = new string[len];
                for (int i = 0; i < len; i++)
                {
                    v[i] = values[i];
                }
                return true;
            }
            return false;
        }

        public bool Got<V>(string name, ref V[] v, uint x = 0) where V : IPersist, new()
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse parse = new JTextParse(str);
                JArr ja = (JArr)parse.Parse();
                int len = ja.Count;
                v = new V[len];
                for (int i = 0; i < len; i++)
                {
                    JObj jo = (JObj)ja[i];
                    V obj = new V();
                    obj.Load(jo, x);
                    v[i] = obj;
                }
                return true;
            }
            return false;
        }

        //
        // HEADER
        //

        public string Header(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                return (string)vs;
            }
            return null;
        }

        public int? HeaderInt(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = (string)vs;
                int v;
                if (int.TryParse(str, out v))
                {
                    return v;
                }
            }
            return null;
        }

        //
        // RESPONSE
        //

        public int StatusCode
        {
            get { return Response.StatusCode; }
            set { Response.StatusCode = value; }
        }

        public void SetHeader(string name, int v)
        {
            Response.Headers.Add(name, new StringValues(v.ToString()));
        }

        public void SetHeader(string name, string v)
        {
            Response.Headers.Add(name, new StringValues(v));
        }

        public void SetHeader(string name, DateTime v)
        {
            string str = StrUtility.ToHttpDate(v);
            Response.Headers.Add(name, new StringValues(str));
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Pub { get; internal set; }

        // the content  is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        public void Out(int status, IContent cont, bool? pub = false, int maxage = 0)
        {
            StatusCode = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Out<T>(int status, T obj, uint x = 0, bool? pub = false, int maxage = 0) where T : IPersist
        {
            Out(status, jcont => jcont.PutObj(obj, x), pub, maxage);
        }

        public void Out<T>(int status, T[] arr, uint x = 0, bool? pub = false, int maxage = 0) where T : IPersist
        {
            Out(status, jcont => jcont.PutArr(arr, x), pub, maxage);
        }

        public void Out(int status, Action<JContent> a, bool? pub = false, int maxage = 0)
        {
            JContent jcont = new JContent(8 * 1024);
            a?.Invoke(jcont);
            Out(status, jcont, pub, maxage);
        }

        public void Out(int status, Action<HtmlContent> a, bool? pub = true, int maxage = 1000)
        {
            HtmlContent hcont = new HtmlContent(8 * 1024);
            a?.Invoke(hcont);
            Out(status, hcont, pub, maxage);
        }

        internal Task WriteContentAsync()
        {
            // setup appropriate headers
            if (Content != null)
            {
                HttpResponse resp = Response;
                resp.ContentLength = Content.Length;
                resp.ContentType = Content.Type;

                if (Pub != null)
                {
                    string cc = Pub.Value ? "public" : "private" + ", max-age=" + MaxAge;
                    SetHeader("Cache-Control", cc);
                }

                // set etag or last-modified 
                if (Content is DynamicContent)
                {
                    long v = ((DynamicContent)Content).ETag;
                    SetHeader("ETag", StrUtility.ToHex(v));
                }
                else
                {
                    DateTime v = ((StaticContent)Content).LastModified;
                    SetHeader("Last-Modified", StrUtility.ToHttpDate(v));
                }

                // send async
                return resp.Body.WriteAsync(Content.Buffer, 0, Content.Length);
            }
            return Task.CompletedTask;
        }

        //
        // RPC
        //

        public async void CallByGet(string service, string part, string uri)
        {
            // token impersonate
            WebClient cli = Control.Service.FindClient(service, part);
            if (cli != null)
            {
                object obj = await cli.GetAsync(uri);
            }
        }

        public void CallByPost(string service, string part, Action<JContent> a)
        {
            // token impersonate
            WebClient cli = Control.Service.FindClient(service, part);
            if (cli != null)
            {
                JContent jcon = new JContent(8 * 1024);
                a?.Invoke(jcon);
            }
        }


        public void Dispose()
        {
            // return request content buffer
            if (bytesSeg != null)
            {
                BufferPool.Return(bytesSeg.Value.Array);
            }

            // return response content buffer
            if (Content is DynamicContent)
            {
                BufferPool.Return(Content.Buffer);
            }
        }

    }
}