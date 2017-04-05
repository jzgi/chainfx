using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public abstract class AbstService : Service<User>, IAuthenticateAsync, ICatch
    {
        public AbstService(ServiceContext sc) : base(sc) { }

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            string token;
            if (ac.Cookies.TryGetValue("Bearer", out token))
            {
                ac.Principal = Decrypt(token);
                return true;
            }

            User prin = null;
            string state = ac.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                string code = ac.Query[nameof(code)];
                if (code == null) return false;
                var atok = await GetAccessTokenAsync(code);
                if (atok.access_token == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(atok.openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Proj.SECRET);
                        prin.stored = true;
                    }
                }
                if (prin == null) // get userinfo remotely
                {
                    prin = await GetUserInfoAsync(atok.access_token, atok.openid);
                }
            }
            else if (ac.ByBrowse)
            {
                // ERR("-------- ByBrowse");
                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic ")) { return true; }

                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(authorization.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string id = orig.Substring(0, colon);
                string password = orig.Substring(colon + 1);
                string md5 = StrUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", (p) => p.Set(id)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Proj.SECRET);
                    }
                }
                // validate
                if (prin == null || !md5.Equals(prin.credential)) { return false; }
            }
            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                ac.SetCookie(prin);
            }
            return true;
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            // ERR("---- Catch() -----");
            // ERR("URI: " + ac.Uri);
            if (e is AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    // ERR("-------- NoToken");
                    // weixin authorization challenge
                    if (ac.ByWeiXin) // weixin
                    {
                        ac.GiveRedirectWeiXinAuthorize();
                    }
                    else // challenge BASIC scheme
                    {
                        ac.SetHeader("WWW-Authenticate", "Basic realm=\"" + Auth.domain + "\"");
                        ac.Give(401); // unauthorized
                    }
                }
                else
                {
                    ac.Give(403); // forbidden
                }
            }
            else
            {
                ac.Give(500, e.Message);
            }
        }
    }
}