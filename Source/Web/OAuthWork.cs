namespace FabricQ.Web
{
    public class OAuthWork : WebWork
    {
        public void authorize(WebContext wc)
        {
            var f = wc.Query;
            string client_id = f[nameof(client_id)];
            string response_type = f[nameof(response_type)];
            string state = f[nameof(state)];
            string redirect_uri = f[nameof(redirect_uri)];
            string scope = f[nameof(scope)];
        }
    }
}