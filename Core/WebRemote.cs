using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A connector to a remote web server.
    ///
    public class WebRemote : IDisposable
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

        byte[] bytes;

        public WebRemote(string raddr)
        {
            string addr = raddr.StartsWith("http") ? raddr : "http://" + raddr;
            client = new HttpClient() { BaseAddress = new Uri(addr) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetHeader(string name, string v)
        {
            request.Headers.Add(name, v);
        }

        public void Send(string uri, IContent cont)
        {

        }

        public async void Get(string uri, Action<FormContent> a)
        {
            if (a != null)
            {
                FormContent cont = new FormContent(false, false, 512);
                a(cont);
                uri = uri + "?" + cont.ToString();
            }
            request = new HttpRequestMessage(HttpMethod.Post, uri);
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        public async void Post<T>(string uri, T cont) where T : HttpContent, IContent
        {
            request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = cont
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        public async void Post(string uri, Action<FormContent> body)
        {
            FormContent cont = null;
            if (body != null)
            {
                cont = new FormContent(true, false, 512);
                body(cont);
            }
            request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = cont
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }


        public async void Post(string uri, Action<JsonContent> body)
        {
            JsonContent cont = null;
            if (body != null)
            {
                cont = new JsonContent(true, true, 8192);
                body(cont);
            }
            request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = cont
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            BufferUtility.Return(cont);
        }

        public void Post(string url, Action<XmlContent> cont)
        {

        }

        //
        // RESPONSE
        //

        async void ReadBytes()
        {
            bytes = await response.Content.ReadAsByteArrayAsync();
        }

        public Obj ReadObj()
        {
            ReadBytes();

            if (bytes == null) return null;
            JsonParse p = new JsonParse(bytes, bytes.Length);
            return (Obj)p.Parse();
        }

        public Arr ReadArr()
        {
            return null;
        }

        public Elem ReadElem()
        {
            return null;
        }

        public D ReadData<D>(byte z = 0) where D : IData, new()
        {
            return default(D);
        }

        public D[] ReadDatas<D>(byte z = 0) where D : IData, new()
        {
            return null;
        }

        public string Header(string name)
        {
            // return response.Headers.TryGetValues
            return null;
        }

        public int StatusCode => (int)response.StatusCode;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}