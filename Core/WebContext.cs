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

        public WebControl Control { get; internal set; }

        public WebAction Action { get; internal set; }

        public IPrincipal Principal { get; internal set; }

        // superscript
        public string Super { get; internal set; }

        //
        // REQUEST
        //
        public string Method => Request.Method;

        public bool IsGet => "GET".Equals(Request.Method);

        public bool IsPost => "POST".Equals(Request.Method);

        // received body bytes
        byte[] buffer; int count;

        // parsed request entity, can be JObj, Form or null
        object entity;

        async void ReceiveAsync()
        {
            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen;
                buffer = BufferPool.Borrow(len);
                count = await req.Body.ReadAsync(buffer, 0, len);
            }
        }

        void ParseEntity()
        {
            if (entity != null) return;

            if (buffer == null) ReceiveAsync();

            if (buffer != null)
            {
                string ctyp = Request.ContentType;
                if ("application/x-www-form-urlencoded".Equals(ctyp))
                {
                    FormParse par = new FormParse(buffer, count);
                    entity = par.Parse();
                }
                else
                {
                    bool jx = "application/jsonx".Equals(ctyp); // json extention
                    JParse par = new JParse(buffer, count, jx);
                    entity = par.Parse();
                }
            }
        }

        public ArraySegment<byte>? BytesSeg
        {
            get
            {
                if (buffer == null) ReceiveAsync();

                if (buffer == null) return null;

                return new ArraySegment<byte>(buffer, 0, count);
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

        public P Obj<P>(byte x = 0xff) where P : IPersist, new()
        {
            ParseEntity();

            ISource src = entity as ISource;
            if (src == null) return default(P);
            P obj = new P();
            obj.Load(src, x);
            return obj;
        }

        public P[] Arr<P>(byte x = 0xff) where P : IPersist, new()
        {
            ParseEntity();

            JArr ja = entity as JArr;
            if (ja == null) return null;
            return ja.ToArr<P>(x);
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

        public bool Got<V>(string name, ref V v, byte x = 0xff) where V : IPersist, new()
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse par = new JTextParse(str);
                JObj jo = (JObj)par.Parse();
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
                JTextParse par = new JTextParse(str);
                v = (JObj)par.Parse();
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
                JTextParse par = new JTextParse(str);
                v = (JArr)par.Parse();
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

        public bool Got<V>(string name, ref V[] v, byte x = 0xff) where V : IPersist, new()
        {
            StringValues values;
            if (Request.Query.TryGetValue(name, out values))
            {
                string str = values[0];
                JTextParse par = new JTextParse(str);
                JArr ja = (JArr)par.Parse();
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

        public DateTime? HeaderDateTime(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = (string)vs;
                DateTime v;
                if (StrUtility.TryParseUtcDate(str, out v))
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
            string str = StrUtility.ToUtcDate(v);
            Response.Headers.Add(name, new StringValues(str));
        }

        public void SetHeader(string name, params string[] values)
        {
            Response.Headers.Add(name, new StringValues(values));
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Pub { get; internal set; }

        // the content  is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        public void Send(int status, IContent cont, bool? pub = null, int maxage = 60000)
        {
            StatusCode = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void SendText(int status, string text, bool? pub = null, int maxage = 60000)
        {
            StatusCode = status;
            Content = new TextContent(text);
            Pub = pub;
            MaxAge = maxage;
        }

        public void SendJ<P>(int status, P obj, byte x = 0xff, bool? pub = null, int maxage = 60000) where P : IPersist
        {
            SendJ(status, cont => cont.PutObj(obj, x), pub, maxage);
        }

        public void SendJ<P>(int status, P[] arr, byte x = 0xff, bool? pub = null, int maxage = 60000) where P : IPersist
        {
            SendJ(status, cont => cont.PutArr(arr, x), pub, maxage);
        }

        public void SendJ(int status, Action<JContent> a, bool? pub = null, int maxage = 60000)
        {
            JContent cont = new JContent(8 * 1024);
            a?.Invoke(cont);
            Send(status, cont, pub, maxage);
        }

        internal async Task SendAsync()
        {
            SetHeader("Connection", "keep-alive");

            if (Pub != null)
            {
                string cc = Pub.Value ? "public" : "private" + ", max-age=" + MaxAge;
                SetHeader("Cache-Control", cc);
            }

            // setup appropriate headers
            if (Content != null)
            {
                HttpResponse r = Response;
                r.ContentLength = Content.Length;
                r.ContentType = Content.Type;

                // cache indicators
                if (Content is DynamicContent) // set etag
                {
                    ulong v = ((DynamicContent)Content).ETag;
                    SetHeader("ETag", StrUtility.ToHex(v));
                }
                else // set last-modified
                {
                    DateTime v = ((StaticContent)Content).LastModified;
                    SetHeader("Last-Modified", StrUtility.ToUtcDate(v));
                }

                // send async
                await r.Body.WriteAsync(Content.Buffer, 0, Content.Length);
            }
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