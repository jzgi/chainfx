using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    ///
    /// The shop operation service.
    ///
    public class ShopService : WebService
    {
        readonly WebAction[] _new;

        public ShopService(WebConfig cfg) : base(cfg)
        {
            MakeVariable<ShopVariableDirectory>();

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
        public void signon(WebActionContext ac)
        {
            if (ac.IsGetMethod) // return the login form
            {
                Form frm = ac.Query;
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];
            }
            else // login
            {
                Form frm = ac.ReadForm();
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                {
                    ac.StatusCode = 400; return; // bad request
                }
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryA("SELECT * FROM shops WHERE id = @1", (p) => p.Put(id)))
                    {
                        var tok = dc.ToData<ShopToken>();
                        string credential = StrUtility.MD5(id + ':' + password);
                        if (credential.Equals(tok.credential))
                        {
                            // set cookie

                            JsonContent cont = new JsonContent(true, false, 256);
                            cont.PutObj(tok);
                            cont.Encrypt(0x4a78be76, 0x1f0335e2);

                            ac.SetHeader("Set-Cookie", "");
                            ac.SetHeader("Location", "");
                            ac.StatusCode = 303; // see other (redirect)
                        }
                        else
                        {
                            ac.StatusCode = 400;
                        }
                    }
                    else
                    {
                        ac.StatusCode = 404;
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
        [CheckAdmin]
        public void @default(WebActionContext ac)
        {
            int page = ac.Sub;
            const byte z = 0xff ^ BIN;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops WHERE NOT disabled ORDER BY id LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                {
                    var shops = dc.ToDatas<Shop>(z);
                    ac.SendHtmlMajor(200, "", main =>
                    {
                        main.form(_new, shops);
                    });
                }
                else
                    ac.SendHtmlMajor(200, "没有记录", main => { });
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
        public void @new(WebActionContext ac)
        {
            if (ac.IsGetMethod)
            {

            }
            else // post
            {
                var shop = ac.ReadData<Shop>(); // read form
                using (var dc = Service.NewDbContext())
                {
                    shop.credential = StrUtility.MD5(shop.id + ':' + ':' + shop.credential);
                    DbSql sql = new DbSql("INSERT INTO users")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(sql.ToString(), p => p.Put(shop)) > 0)
                    {
                        ac.StatusCode = 201; // created
                    }
                    else
                        ac.StatusCode = 500; // internal server error
                }
            }
        }


        protected override IPrincipal Principalize(string token)
        {
            string plain = StrUtility.Decrypt(token, 0x4a78be76, 0x1f0335e2); // plain token
            JsonParse par = new JsonParse(plain);
            try
            {
                Obj obj = (Obj)par.Parse();
                return obj.ToData<ShopToken>();
            }
            catch
            {
            }
            return null;
        }

        [CheckAdmin]
        public virtual void mgmt(WebActionContext ac)
        {
            if (Children != null)
            {
                ac.SendHtmlMajor(200, "模块管理", a =>
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            WebDirectory child = Children[i];
                        }
                    },
                    true);
            }
        }


        public void report(WebActionContext ac)
        {
            using (var call = ac.NewWebCall())
            {

            }
        }
    }
}