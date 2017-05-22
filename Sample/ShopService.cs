using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public class ShopService : Service<User>, IAuthenticateAsync, ICatch
    {
        readonly City[] cities;

        readonly string[] cityopt;

        public ShopService(ServiceContext sc) : base(sc)
        {
            Create<PubShopWork>("pub"); // public

            Create<MyUserWork>("my"); // personal

            Create<OprShopWork>("opr"); // shop operator

            Create<SprCityWork>("spr"); // local supervisor

            Create<AdmWork>("adm"); // administrator

            cities = DataInputUtility.FileToDatas<City>(sc.GetFilePath("$cities.json"));

            int len = cities.Length;
            cityopt = new string[len];
            for (int i = 0; i < len; i++)
            {
                cityopt[i] = cities[i].name;
            }
        }

        public string[] CityOpt => cityopt;

        public string[] GetDistrs(string city) => cities.Find(e => e.name == city)?.distrs;

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            string token;
            if (ac.Cookies.TryGetValue("Token", out token))
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
                var accessor = await GetAccessorAsync(code);
                if (accessor.access_token == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(accessor.openid)))
                    {
                        prin = dc.ToData<User>(-1 ^ User.CREDENTIAL);
                    }
                }
                if (prin == null) // get userinfo remotely
                {
                    prin = await GetUserInfoAsync(accessor.access_token, accessor.openid);
                }
            }
            else if (ac.ByBrowse)
            {
                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic "))
                {
                    return true;
                }

                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(authorization.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string id = orig.Substring(0, colon);
                string credential = StrUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(id)))
                    {
                        prin = dc.ToData<User>(-1);
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return false;
                }
            }
            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
            }
            return true;
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            if (e is AuthorizeException)
            {
                if (ac.Principal == null)
                {
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
                    ac.Give(403, "您没有访问权限"); // forbidden
                }
            }
            else
            {
                ac.Give(500, e.Message);
            }
        }

        public void @default(ActionContext ac)
        {
            DoFile("default.html", ".html", ac);
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();

            long orderid;
            decimal cash;
            if (Notified(xe, out orderid, out cash))
            {
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orders SET cash = @1, accepted = localtimestamp, status = @2 WHERE id = @3 AND status <= @2", (p) => p.Set(cash).Set(Order.ACCEPTED).Set(orderid));
                }
                // return xml
                XmlContent cont = new XmlContent(true, 1024);
                cont.ELEM("xml", null, () =>
                {
                    cont.ELEM("return_code", "SUCCESS");
                    cont.ELEM("return_msg", "OK");
                });
                ac.Give(200, cont);
            }
            else
            {
                ac.Give(304);
            }
        }
    }
}