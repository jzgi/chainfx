using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static ChainFX.DataUtility;

namespace ChainFX.Web;

/// <summary>
/// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
/// </summary>
public class WebConnect : IKeyable<string>
{
    public const string
        CONTENT_TYPE = "Content-Type",
        CONTENT_LENGTH = "Content-Length",
        COOKIE = "Cookie",
        MAP_ADDRESS = "Map-Address";


    protected readonly HttpClientHandler handler;

    protected readonly HttpClient client;


    public string Key { get; set; }


    /// <summary>
    /// Used to construct a random client that does not necessarily connect to a remote service. 
    /// </summary>
    public WebConnect(string baseUri, string file = null, string password = null)
    {
        handler = new HttpClientHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ClientCertificateOptions = ClientCertificateOption.Manual
        };
        if (file != null && password != null)
        {
            var cert = new X509Certificate2(file, password, X509KeyStorageFlags.MachineKeySet);
            handler.ClientCertificates.Add(cert);
        }
        client = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUri),
            Timeout = TimeSpan.FromSeconds(6), // default to 6 seconds
        };
    }

    public Uri BaseAddress => client.BaseAddress;

    //
    // RPC
    //

    public async Task<(short, S)> GetAsync<S>(string uri, Action<HttpRequestHeaders> headers = null) where S : class, ISource
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            headers?.Invoke(req.Headers);
            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                var src = (S)ParseContent(ctyp, bytea, bytea.Length, typeof(S));

                return ((short)rsp.StatusCode, src);
            }
            return ((short)rsp.StatusCode, default);
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return (0, default);
        }
    }

    public async Task<(short, D)> GetObjectAsync<D>(string uri, short msk = 0xff, Action<HttpRequestHeaders> headers = null) where D : IData, new()
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            headers?.Invoke(req.Headers);
            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                var inp = ParseContent(ctyp, bytea, bytea.Length);

                var obj = new D();
                obj.Read(inp, msk);

                return ((short)rsp.StatusCode, obj);
            }
            return ((short)rsp.StatusCode, default);
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return (0, default);
        }
    }

    public async Task<(short, D[])> GetArrayAsync<D>(string uri, short msk = 0xff, Action<HttpRequestHeaders> headers = null) where D : IData, new()
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            headers?.Invoke(req.Headers);
            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);

                var inp = ParseContent(ctyp, bytea, bytea.Length);
                var arr = inp.ToArray<D>(msk);

                return ((short)rsp.StatusCode, arr);
            }
            return ((short)rsp.StatusCode, default);
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return (0, default);
        }
    }

    public async Task<short> PostAsync(string uri, IContent content, Action<HttpRequestHeaders> headers = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            headers?.Invoke(req.Headers);
            req.Content = (HttpContent)content;
            req.Headers.TryAddWithoutValidation(CONTENT_TYPE, content.CType);
            req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, content.Count.ToString());
            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return (short)rsp.StatusCode;
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return 0;
        }
        finally
        {
            if (content is ContentBuilder cnt)
            {
                cnt.Clear();
            }
        }
    }

    public async Task<(short, S)> PostAsync<S>(string uri, IContent content, Action<HttpRequestHeaders> headers = null) where S : class, ISource
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            headers?.Invoke(req.Headers);
            req.Content = (HttpContent)content;
            req.Headers.TryAddWithoutValidation(CONTENT_TYPE, content.CType);
            req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, content.Count.ToString());

            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK || rsp.StatusCode == HttpStatusCode.Created)
            {
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                var bytes = await rsp.Content.ReadAsByteArrayAsync();
                var model = ParseContent(ctyp, bytes, bytes.Length, typeof(S)) as S;

                return ((short)rsp.StatusCode, model);
            }
            return ((short)rsp.StatusCode, null);
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return (0, default);
        }
        finally
        {
            if (content is ContentBuilder cnt)
            {
                cnt.Clear();
            }
        }
    }
}