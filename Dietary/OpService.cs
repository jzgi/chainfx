using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Ministry.Dietary
{
    ///
    /// The operation service.
    ///
    public class OpService : WebService
    {
        public OpService(WebConfig cfg) : base(cfg)
        {
            SetVariable<ShopVariableDir>();
        }

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        public void @default(WebContext wc, int page)
        {
            const byte z = 0xff ^ BIN;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops WHERE NOT disabled ORDER BY id LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                {
                    var shops = dc.ToDatas<Shop>(z);
                    wc.SendMajorLayout(200, "", main => { });
                }
                else
                    wc.SendMajorLayout(200, "没有记录", main => { });
            }
        }

        /// Create a new shop
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        [CheckAdmin]
        public void @new(WebContext wc, string subscpt)
        {
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
                wc.SendMajorLayout(200, "模块管理", a =>
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