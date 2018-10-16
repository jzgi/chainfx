using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// To determine principal identity based on current web context. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UserAuthenticateAttribute : AuthenticateAttribute
    {
        const string WXAUTH = "wxauth";

        public override bool Do(WebContext wc) => throw new NotImplementedException();

        public UserAuthenticateAttribute() : base(true)
        {
        }

        public override async Task<bool> DoAsync(WebContext wc)
        {
            // if principal already in cookie
            if (wc.Cookies.TryGetValue("Token", out var token))
            {
                wc.Principal = wc.Service.Decrypt<User>(token);
                return true;
            }

            // resolve principal thru OAuth2 or HTTP-basic
            User prin = null;
            string state = wc.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                string code = wc.Query[nameof(code)];
                if (code == null)
                {
                    return false;
                }
                string hubid = wc[0];
                var hub = wc.Work.Obtain<Map<string, Hub>>()[hubid];
                (_, string openid) = await hub.GetAccessorAsync(code);
                if (openid == null)
                {
                    return false;
                }
                // check in db
                using (var dc = wc.Service.NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", p => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(0xff ^ User.PRIVACY);
                    }
                    else
                    {
                        prin = new User {wx = openid}; // create a tempary principal
                    }
                }
            }
            else
            {
                string h_auth = wc.Header("Authorization");
                if (h_auth == null || !h_auth.StartsWith("Basic "))
                {
                    return true;
                }
                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(h_auth.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string tel = orig.Substring(0, colon);
                string credential = TextUtility.MD5(orig);
                using (var dc = wc.Service.NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", p => p.Set(tel)))
                    {
                        prin = dc.ToObject<User>();
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return true;
                }
            }

            // setup principal and cookie
            if (prin != null)
            {
                // set token success
                wc.Principal = prin;
                if (!prin.IsTemporary)
                {
                    wc.SetTokenCookie(prin, 0xff ^ User.PRIVACY);
                }
            }
            return true;
        }
    }
}