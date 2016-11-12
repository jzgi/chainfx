using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Ministry.Dietary
{
    ///
    /// The operation service.
    ///
    public class ShopService : WebService
    {
        readonly WebAction[] _new;

        public ShopService(WebConfig cfg) : base(cfg)
        {
            SetMux<ShopMuxDir>();

            _new = GetActions(nameof(@new));
        }

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        [CheckAdmin]
        public void @default(WebContext wc, int page)
        {
            const byte z = 0xff ^ BIN;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops WHERE NOT disabled ORDER BY id LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                {
                    var shops = dc.ToDatas<Shop>(z);
                    wc.SendHtmlMajor(200, "", main =>
                    {
                        main.form(_new, shops);
                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
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
        public void @new(WebContext wc)
        {
            if (wc.IsGetMethod)
            {

            }
            else // post
            {
                var shop = wc.ReadData<Shop>(); // read form
                using (var dc = Service.NewDbContext())
                {
                    shop.credential = StrUtility.MD5(shop.id + ':' + ':' + shop.credential);
                    DbSql sql = new DbSql("INSERT INTO users")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(sql.ToString(), p => p.Put(shop)) > 0)
                    {
                        wc.StatusCode = 201; // created
                    }
                    else
                        wc.StatusCode = 500; // internal server error
                }

            }
        }

        //
        // MESSAGES
        // 

        public void USER_UPD(MsgContext mc)
        {
        }

        public void RPT_OK(MsgContext mc)
        {
        }


        protected override IPrincipal Fetch(bool token, string idstr)
        {
            if (token) // token
            {
                string plain = StrUtility.Decrypt(idstr, 0x4a78be76, 0x1f0335e2); // plain token
                JsonParse par = new JsonParse(plain);
                try
                {
                    Obj jo = (Obj)par.Parse();
                    // return jo.ToObj<Token>();
                }
                catch
                {
                }
            }
            else // username
            {
                // if (logins != null)
                // {
                //     for (int i = 0; i < logins.Length; i++)
                //     {
                //         Login lgn = logins[i];
                //         if (lgn.id.Equals(idstr)) return lgn;
                //     }
                // }
            }
            return null;
        }

        [CheckAdmin]
        public virtual void mgmt(WebContext wc, string subscpt)
        {
            if (Children != null)
            {
                wc.SendHtmlMajor(200, "模块管理", a =>
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            WebDir child = Children[i];
                        }
                    },
                    true);
            }
        }
    }
}