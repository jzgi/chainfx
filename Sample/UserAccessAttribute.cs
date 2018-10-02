using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or action method. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UserAccessAttribute : AccessAttribute
    {
        // required hub access
        readonly short hubly;

        // required shop access
        readonly short shoply;

        // required customer team access
        readonly short teamly;

        public UserAccessAttribute(short hubly = 0, short shoply = 0, short teamly = 0) : base(1)
        {
            this.hubly = hubly;
            this.shoply = shoply;
            this.teamly = teamly;
        }

        public override bool Authenticate(WebContext wc) => throw new NotImplementedException();

        public override async Task<bool> AuthenticateAsync(WebContext wc)
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
            if (Hub.WXAUTH.Equals(state)) // if weixin auth
            {
                string code = wc.Query[nameof(code)];
                if (code == null)
                {
                    return false;
                }
                string hubid = wc[0];
                wc.Work.INF("hubid := " + hubid);
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
                        prin = new User { wx = openid }; // create a minimal principal object
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
                wc.SetTokenCookie(prin, 0xff ^ User.PRIVACY);
            }
            return true;
        }


        public override bool Authorize(WebContext wc)
        {
            var o = (User)wc.Principal;

            if (o == null || o.IsIncomplete) return false;

            // if requires hub access
            if (hubly > 0)
            {
                return (o.hubly & hubly) > 0;
            }
            // if requires shop access
            if (shoply > 0)
            {
                if ((o.teamly & shoply) != shoply) return false; // inclusive check
                short at = wc[typeof(IOrgVar)];
                if (at != 0)
                {
                    return o.shopat == at;
                }
                return true;
            }
            // if requires customer team access
            if (teamly > 0)
            {
                if ((o.teamly & teamly) != teamly) return false; // inclusive check
                short at = wc[typeof(IOrgVar)];
                if (at != 0)
                {
                    return o.teamat == at;
                }
                return true;
            }
            return true;
        }
    }
}