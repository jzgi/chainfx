using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static ChainFx.DataUtility;

namespace ChainFx.Web;

/// <summary>
/// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
/// </summary>
public class WebConnect : IKeyable<string>
{
    public const string
        CONTENT_TYPE = "Content-Type",
        CONTENT_LENGTH = "Content-Length",
        COOKIE = "Cookie";


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

    public async Task<(short, M)> GetAsync<M>(string uri, string token = null) where M : class, ISource
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (token != null)
            {
                req.Headers.TryAddWithoutValidation(COOKIE, "token=" + token);
            }
            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                var model = (M)ParseContent(ctyp, bytea, bytea.Length, typeof(M));

                return ((short)rsp.StatusCode, model);
            }
            return ((short)rsp.StatusCode, default);
        }
        catch (Exception e)
        {
            Application.Err(e.Message);

            return (0, default);
        }
    }

    public async Task<(short, D)> GetObjectAsync<D>(string uri, short msk = 0xff, string token = null) where D : IData, new()
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (token != null)
            {
                req.Headers.TryAddWithoutValidation(COOKIE, "token=" + token);
            }
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

    public async Task<(short, D[])> GetArrayAsync<D>(string uri, short msk = 0xff, string token = null) where D : IData, new()
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (token != null)
            {
                req.Headers.TryAddWithoutValidation(COOKIE, "token=" + token);
            }
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

    public async Task<short> PostAsync(string uri, IContent content, string token = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (token != null)
            {
                req.Headers.TryAddWithoutValidation(COOKIE, "token=" + token);
            }
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

    public async Task<(short, M)> PostAsync<M>(string uri, IContent content, string token = null) where M : class, ISource
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (token != null)
            {
                req.Headers.TryAddWithoutValidation(COOKIE, "token=" + token);
            }
            req.Content = (HttpContent)content;
            req.Headers.TryAddWithoutValidation(CONTENT_TYPE, content.CType);
            req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, content.Count.ToString());

            var rsp = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode == HttpStatusCode.OK || rsp.StatusCode == HttpStatusCode.Created)
            {
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                var bytes = await rsp.Content.ReadAsByteArrayAsync();
                var model = ParseContent(ctyp, bytes, bytes.Length, typeof(M)) as M;

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