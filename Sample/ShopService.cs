using System.Threading.Tasks;
using Greatbone.Core;
using NpgsqlTypes;
using static Greatbone.Core.FlagsUtility;

namespace Greatbone.Sample
{
    ///
    /// The shop operation service.
    ///
    public class ShopService : WebService
    {
        static readonly WebClient WeiXin = new WebClient("wechat", "http://sh.api.weixin.qq.com");

        readonly WebAction[] _new;

        public ShopService(WebConfig cfg) : base(cfg)
        {
            MakeVariable<ShopVariableFolder>();

            _new = GetActions(nameof(@new));
        }

        ///
        /// redirect_uri/?code=CODE&amp;state=STATE
        public async Task weixin(WebActionContext ac)
        {
            string code = ac[nameof(code)];
            if (code == null)
            {
                // redirect the user to weixin authorization page
                ac.SetHeader("Location", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=SCOPE&state=STATE#wechat_redirect");
                ac.Reply(302);
            }
            else
            {
                // get access token by the code
                JObj jo = await WeiXin.GetAsync<JObj>(null, "https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code");

                string access_token = jo[nameof(access_token)];
                string openid = jo[nameof(openid)];

                // get user info

                jo = await WeiXin.GetAsync<JObj>(null, "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                string nickname = jo[nameof(nickname)];

                // display shop list form
                
            }


        }


        ///
        /// Get the singon form or perform a signon action.
        ///
        /// <code>
        /// GET /signon[id=_id_&amp;password=_password_&amp;orig=_orig_]
        /// </code>
        ///
        /// <code>
        /// POST /signon
        ///  
        /// id=_id_&amp;password=_password_[&amp;orig=_orig_]
        /// </code>
        ///
        public async Task signon(WebActionContext ac)
        {
            if (ac.GET) // return the login form
            {
                Form frm = ac.Query;
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];

                ac.ReplyPage(200, "", main => { main.FORM(null, x => x.INPUT_button()); });
            }
            else // login
            {
                Form frm = await ac.ReadAsync<Form>();
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                {
                    ac.Reply(400);
                    return; // bad request
                }
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryUn("SELECT * FROM shops WHERE id = @1", (p) => p.Set(id)))
                    {
                        var tok = dc.ToUn<Token>();
                        string credential = TextUtility.MD5(id + ':' + password);
                        if (credential.Equals(tok.credential))
                        {
                            // set cookie
                            string tokstr = Service.Auth.Encrypt(tok);
                            ac.SetHeader("Set-Cookie", tokstr);
                            ac.SetHeader("Location", "");
                            ac.Reply(303); // see other (redirect)
                        }
                        else
                        {
                            ac.Reply(400);
                        }
                    }
                    else
                    {
                        ac.Reply(404);
                    }
                }
            }
        }

        public void test(WebActionContext ac)
        {
        }

        ///
        /// Get nearest shops
        ///
        /// <code>
        /// GET /nearest?pt=x,y
        /// </code>
        ///
        public void nearest(WebActionContext ac)
        {
            NpgsqlPoint pt = ac[nameof(pt)];

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty)._("FROM shops WHERE location <-> @1");
                if (dc.Query(sql.ToString(), p => p.Set(pt)))
                {
                    var shops = dc.ToArray<Shop>();
                    ac.ReplyPage(200, "", main => { main.FORM(_new, shops); });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        #region MANAGEMENT

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        [CheckAdmin]
        public void @default(WebActionContext ac)
        {
            GetUiActions(typeof(CheckAdminAttribute));

            ac.ReplyPage(200, "", x =>
            {
                // x.Form();
            });
        }

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        public void mgmtz(WebActionContext ac, string arg)
        {
            int page = arg.ToInt();
            const byte z = 0xff ^ BINARY;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops");
                if (dc.Query(sql.ToString(), p => p.Set(20 * page)))
                {
                    var shops = dc.ToArray<Shop>(z);
                    ac.ReplyPage(200, "", main => { main.FORM(_new, shops); });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        /// Create a new shop
        ///
        /// <code>
        /// GET /new
        /// </code>
        ///
        /// <code>
        /// POST /new
        ///
        /// id=_shopid_&amp;password=_password_&amp;name=_name_
        /// </code>
        ///
        [CheckAdmin]
        public async Task @new(WebActionContext ac)
        {
            if (ac.GET)
            {
            }
            else // post
            {
                var shop = await ac.ReadUnAsync<Shop>(); // read form
                using (var dc = Service.NewDbContext())
                {
                    shop.credential = TextUtility.MD5(shop.id + ':' + shop.credential);
                    DbSql sql = new DbSql("INSERT INTO users")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(sql.ToString(), p => p.Set(shop)) > 0)
                    {
                        ac.Reply(201); // created
                    }
                    else
                        ac.Reply(500); // internal server error
                }
            }
        }


        [CheckAdmin]
        public virtual void mgmt(WebActionContext ac)
        {
            if (Children != null)
            {
                ac.ReplyPage(200, "模块管理", a =>
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            WebFolder child = Children[i];
                        }
                    },
                    true);
            }
        }


        public async Task report(WebActionContext ac)
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}