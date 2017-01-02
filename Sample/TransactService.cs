using Greatbone.Core;
using NpgsqlTypes;
using static Greatbone.Core.BitsUtility;

namespace Greatbone.Sample
{
    ///
    /// The shop operation service.
    ///
    public class TransactService : WebService
    {
        static readonly WebClient WeChat = new WebClient("wechat", "http://sh.api.weixin.qq.com");

        readonly WebAction[] _new;

        public TransactService(WebConfig cfg) : base(cfg)
        {
            MakeVariable<ShopVariableFolder>();

            _new = GetActions(nameof(@new));
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
        public async void signon(WebActionContext ac)
        {
            if (ac.GET) // return the login form
            {
                Form frm = ac.Query;
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];

                ac.ReplyPage(200, "", main =>
                {
                    main.FORM(null, x => x.INPUT_button());
                });

            }
            else // login
            {
                Form frm = await ac.GetFormAsync();
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                {
                    ac.Reply(400); return; // bad request
                }
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryA("SELECT * FROM shops WHERE id = @1", (p) => p.Put(id)))
                    {
                        var tok = dc.ToDat<Token>();
                        string credential = StrUtility.MD5(id + ':' + password);
                        if (credential.Equals(tok.credential))
                        {
                            // set cookie
                            string tokstr = Service.Auth.Encrypt(tok);
                            ac.Header("Set-Cookie", tokstr);
                            ac.Header("Location", "");
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

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        public void @default(WebActionContext ac)
        {
            int page = ac.Arg;
            const byte z = 0xff ^ BINARY;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops");
                if (dc.Query(sql, p => p.Put(20 * page)))
                {
                    var shops = dc.ToDats<Shop>(z);
                    ac.ReplyPage(200, "", main =>
                    {
                        main.FORM(_new, shops);
                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
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
                if (dc.Query(sql.ToString(), p => p.Put(pt)))
                {
                    var shops = dc.ToDats<Shop>();
                    ac.ReplyPage(200, "", main =>
                    {
                        main.FORM(_new, shops);
                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        #region MANAGEMENT


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
        [Admin]
        public async void @new(WebActionContext ac)
        {
            if (ac.GET)
            {

            }
            else // post
            {
                var shop = await ac.GetDatAsync<Shop>(); // read form
                using (var dc = Service.NewDbContext())
                {
                    shop.credential = StrUtility.MD5(shop.id + ':' + shop.credential);
                    DbSql sql = new DbSql("INSERT INTO users")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(sql.ToString(), p => p.Put(shop)) > 0)
                    {
                        ac.Reply(201); // created
                    }
                    else
                        ac.Reply(500); // internal server error
                }
            }
        }


        [Admin]
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


        public void report(WebActionContext ac)
        {

        }

        #endregion
    }
}