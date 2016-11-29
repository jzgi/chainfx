using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A connector to a remote web server.
    ///
    public class WebRemote: IDisposable
    {
        // remote address
        readonly string raddr;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        HttpRequestMessage request;

        IContent content;

        HttpResponseMessage response;

        public WebRemote(string raddr)
        {
            this.raddr = raddr;
            client = new HttpClient() { BaseAddress = new Uri("http://" + raddr) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetHeader(string name, string v)
        {
            request.Headers.Add(name, v);
        }

        public void Send(string uri, IContent cont)
        {

        }

        public void SendJsonGet(string uri, Action<JsonContent> a)
        {
            // JsonContent cont = new JsonContent();
            // a(cont);
            // caller
            
        }

        public void GetXml(string url, Action<XmlContent> cont)
        {

        }

        public void PostJson(string url, Action<JsonContent> cont)
        {

        }

        public void PostXml(string url, Action<XmlContent> cont)
        {

        }

        public void PostForm(string url, Action<FormContent> cont)
        {

        }


        //
        // RESPONSE
        //

        public Obj ReadJObj()
        {
            return null;
        }

        public Arr ReadJArr()
        {
            return null;
        }

        public B ReadObj<B>(byte z = 0) where B : IData, new()
        {
            return default(B);
        }

        public B[] ReadArr<B>(byte z = 0) where B : IData, new()
        {
            return null;
        }

        public string Header(string name)
        {
            // return response.Headers.TryGetValues
            return null;
        }

        public int StatusCode => (int)response.StatusCode;

        public async Task<object> GetAsync(string uri)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            HttpResponseMessage response = await client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            JsonParse par = new JsonParse(bytes, bytes.Length);
            return par.Parse();
        }

        public async Task<HttpResponseMessage> PostXmlAsync(string uri, Action<XmlContent> content)
        {
            XmlContent cont = new XmlContent(true, true);
            content?.Invoke(cont);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                // Content = new WebRemote(cont)
            };

            return await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public async Task<HttpResponseMessage> PostJAsync(string uri, Action<JsonContent> content)
        {
            JsonContent cont = new JsonContent(true, true);
            content?.Invoke(cont);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                // Content = new WebCall(cont)
            };
            return await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public Elem GetElemAsync(string uri)
        {
            return null;
        }

        public Obj GetJObjAsync(string uri)
        {
            return null;
        }

        public Arr GetJArrAsync(string uri)
        {
            return null;
        }

        public byte[] GetBytesAsync(string uri)
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}