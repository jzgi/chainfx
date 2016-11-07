using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// The encapsulation of a web request/response exchange context.
    /// </summary>
    ///
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

        public bool IsGetMethod => "GET".Equals(Request.Method);

        public bool IsPostMethod => "POST".Equals(Request.Method);

        Form query;


        // received request body
        byte[] bytebuf;

        // number of received bytes
        int count;

        // parsed request entity (JObj, JArr, Form, null)
        object entity;

        async void ReadAsync()
        {
            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen;
                bytebuf = BufferUtility.GetByteBuffer(len);
                count = await req.Body.ReadAsync(bytebuf, 0, len);
            }
        }

        public bool IsPooled => bytebuf != null;


        public Form Query
        {
            get
            {
                if (query == null)
                {
                    string qstr = Request.QueryString.Value;
                    if (!string.IsNullOrEmpty(qstr))
                    {
                        FormParse par = new FormParse(qstr);
                        query = par.Parse();
                    }
                }
                return query;
            }
        }

        void ParseEntity()
        {
            if (entity != null) return;

            if (bytebuf == null) ReadAsync();

            if (bytebuf != null)
            {
                string ctyp = Request.ContentType;
                if ("application/x-www-form-urlencoded".Equals(ctyp))
                {
                    FormParse par = new FormParse(bytebuf, count);
                    entity = par.Parse();
                }
                else
                {
                    bool jx = "application/jsonx".Equals(ctyp); // json extention
                    JParse par = new JParse(bytebuf, count, jx);
                    entity = par.Parse();
                }
            }
        }

        public ArraySegment<byte>? ReadBytesSeg()
        {
            if (bytebuf == null) ReadAsync();

            if (bytebuf == null) return null;

            return new ArraySegment<byte>(bytebuf, 0, count);
        }

        public Form ReadForm()
        {
            ParseEntity();
            return entity as Form;
        }

        public JObj ReadJObj()
        {
            ParseEntity();
            return entity as JObj;
        }

        public JArr ReadJArr()
        {
            ParseEntity();
            return entity as JArr;
        }

        public P ReadObj<P>(byte z = 0) where P : IPersist, new()
        {
            ParseEntity();

            ISource src = entity as ISource;
            if (src == null) return default(P);
            P obj = new P();
            obj.Load(src, z);
            return obj;
        }

        public P[] ReadArr<P>(byte z = 0) where P : IPersist, new()
        {
            ParseEntity();

            JArr ja = entity as JArr;
            if (ja == null) return null;
            return ja.ToArr<P>(z);
        }

        //
        // SOURCE FOR QUERY STRING
        //

        public bool Get(string name, ref bool v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref short v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref int v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref long v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref decimal v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref Number v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref DateTime v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref char[] v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref string v)
        {

            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get<V>(string name, ref V v, byte z = 0) where V : IPersist, new()
        {
            return Query == null ? false : Query.Get(name, ref v, z);
        }

        public bool Get(string name, ref JObj v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref JArr v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref short[] v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref int[] v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref long[] v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get(string name, ref string[] v)
        {
            return Query == null ? false : Query.Get(name, ref v);
        }

        public bool Get<V>(string name, ref V[] v, byte z = 0) where V : IPersist, new()
        {
            return Query == null ? false : Query.Get(name, ref v, z);
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
            string str = StrUtility.FormatUtcDate(v);
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
            Content = new PlainContent(true, true)
            {
                Text = text
            };

            Pub = pub;
            MaxAge = maxage;
        }

        public void SendJ<P>(int status, P obj, byte z = 0, bool? pub = null, int maxage = 60000) where P : IPersist
        {
            SendJ(status, cont => cont.PutObj(obj, z), pub, maxage);
        }

        public void SendJ<P>(int status, P[] arr, byte z = 0, bool? pub = null, int maxage = 60000) where P : IPersist
        {
            SendJ(status, cont => cont.PutArr(arr, z), pub, maxage);
        }

        public void SendJ(int status, Action<JContent> a, bool? pub = null, int maxage = 60000)
        {
            JContent cont = new JContent(true, true, 4 * 1024);
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
                r.ContentLength = Content.Size;
                r.ContentType = Content.Type;

                // cache indicators
                if (Content is DynamicContent) // set etag
                {
                    ulong etag = ((DynamicContent)Content).ETag;
                    SetHeader("ETag", StrUtility.ToHex(etag));
                }

                // set last-modified
                DateTime? last = Content.Modified;
                if (last != null)
                {
                    SetHeader("Last-Modified", StrUtility.FormatUtcDate(last.Value));
                }

                // send async
                await r.Body.WriteAsync(Content.ByteBuffer, 0, Content.Size);
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
                JContent cont = new JContent(true, true, 8 * 1024);
                a?.Invoke(cont);
                BufferUtility.Return(cont);

            }
        }


        public void Dispose()
        {
            // return request content buffer
            if (IsPooled)
            {
                BufferUtility.Return(bytebuf);
            }

            // return response content buffer
            if (Content != null && Content.IsPooled)
            {
                BufferUtility.Return(Content.ByteBuffer);
            }
        }

    }

}