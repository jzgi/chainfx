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
        // required reg access
        readonly short reg;

        // required shop access
        readonly short shop;

        // required customer team access
        readonly short team;

        public UserAccessAttribute(short reg = 0, short shop = 0, short team = 0) : base(1)
        {
            this.reg = reg;
            this.shop = shop;
            this.team = team;
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
            if (Reg.WXAUTH.Equals(state)) // if weixin auth
            {
                string code = wc.Query[nameof(code)];
                if (code == null)
                {
                    return false;
                }
                string regid = wc[0];
                var reg = wc.Work.Obtain<Map<string, Reg>>()[regid];
                (_, string openid) = await reg.GetAccessorAsync(code);

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
                        prin = new User {wx = openid}; // create a minimal principal object
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
            var o = (User) wc.Principal;

            if (o == null || o.IsIncomplete) return false;

            // if requires hub access
            if (reg > 0)
            {
                return (o.regly & reg) > 0;
            }
            // if requires shop access
            if (shop > 0)
            {
                if ((o.teamly & shop) != shop) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.shopat == at;
                }
                return true;
            }
            // if requires customer team access
            if (team > 0)
            {
                if ((o.teamly & team) != team) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.teamat == at;
                }
                return true;
            }
            return true;
        }
    }
}