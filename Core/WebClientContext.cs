using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A single request/response eachange or a seris of them.
    ///
    public class WebClientContext : IDisposable
    {
        readonly WebClient client;

        HttpRequestMessage request;

        // for whenall operation
        internal Task<HttpResponseMessage> task;

        HttpResponseMessage response;

        public WebClientContext(WebClient client)
        {
            this.client = client;
        }

        public WebActionContext ActionContext { get; set; }

        void Reset()
        {
            if (request != null)
            {
                // recyle the byte buffer
                IContent c = request.Content as IContent;
                if (c != null)
                {
                    BufferUtility.Return(c);
                }
                // release resources
                request.Dispose();
            }
            if (response != null)
            {
                response.Dispose();
                response = null;
            }
        }

        void InitRequest()
        {
            if (ActionContext != null)
            {
                // bearer

                // cookie 
            }

        }

        public void Get(string uri)
        {
            Get(uri, null);
        }

        public void Get(string path, Action<FormContent> query)
        {
            Reset();

            string uri = path;
            if (query != null)
            {
                FormContent cont = new FormContent(false, false, 512);
                query(cont);
                uri = path + "?" + cont.ToString();
            }

            request = new HttpRequestMessage(HttpMethod.Get, uri);
            InitRequest();

            task = client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        public void PostJson(string uri, Action<JsonContent> body)
        {
            Reset();

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
            InitRequest();

            task = client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        public void SetHeader(string name, string v)
        {
            request.Headers.Add(name, v);
        }

        public async void Post<T>(string uri, T cont) where T : HttpContent, IContent
        {
            Reset();

            request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = cont
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        public async void PostForm(string uri, Action<FormContent> body)
        {
            Reset();

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


        public async void PostXml(string uri, Action<XmlContent> body)
        {
            Reset();

            XmlContent cont = null;
            if (body != null)
            {
                cont = new XmlContent(true, true, 8192);
                body(cont);
            }
            request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = cont
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            BufferUtility.Return(cont);
        }

        //
        // RESPONSE
        //

        public async Task<int> StatusCode()
        {
            if (response == null)
            {
                response = await task;
            }
            return (int)response.StatusCode;
        }

        public async Task<Obj> ReadObj()
        {
            if (response == null)
            {
                response = await task;
            }
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null)
            {
                return null;
            }
            JsonParse p = new JsonParse(bytes, bytes.Length);
            return (Obj)p.Parse();
        }

        public async Task<Arr> ReadArr()
        {
            if (response == null)
            {
                response = await task;
            }

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null)
            {
                return null;
            }
            JsonParse p = new JsonParse(bytes, bytes.Length);
            return (Arr)p.Parse();
        }

        public Elem ReadElem()
        {
            return null;
        }

        public async Task<D> ReadData<D>(byte z = 0) where D : IData, new()
        {
            if (response == null)
            {
                response = await task;
            }

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null)
            {
                return default(D);
            }
            JsonParse p = new JsonParse(bytes, bytes.Length);
            Obj obj = (Obj)p.Parse();
            return obj.ToData<D>(z);
        }

        public async Task<D[]> ReadDatas<D>(byte z = 0) where D : IData, new()
        {
            if (response == null)
            {
                response = await task;
            }

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes == null)
            {
                return null;
            }
            JsonParse p = new JsonParse(bytes, bytes.Length);
            Arr arr = (Arr)p.Parse();
            return arr.ToDatas<D>(z);
        }

        public string Header(string name)
        {
            return null;
        }

        public int HeaderInt(string name)
        {
            return 0;
        }

        public DateTime HeaderDateTime(string name)
        {
            return default(DateTime);
        }

        public void Dispose()
        {
            Reset();
        }
    }
}