using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>
    /// The user variable-hub controller.
    ///
    public class UserMultiple : WebMultiple
    {
        public UserMultiple(WebArg arg) : base(arg)
        {
        }

        /// <summary>
        /// Get a user token.
        /// </summary>
        /// <code>
        /// GET /user/_id_/?password=_password_
        /// </code>
        public override void @default(WebContext wc, string sub)
        {
            string id = wc.Super;
            string password = null;
            if (!wc.Got(nameof(password), ref password))
            {
                wc.StatusCode = 400; return;
            }
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM users WHERE id = @1", (p) => p.Put(id)))
                {
                    Token obj = new Token();
                    obj.Load(dc);
                    string c16 = StrUtility.C16(password);
                    if (c16.Equals(obj.credential))
                    {
                        JText t = new JText();
                        obj.Save(t);
                        string tok = Token.Encrypt(t.ToString());

                        wc.Send(200, tok);
                    }
                    else
                    {
                        wc.StatusCode = 400;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Update a profile indicated by token.
        /// </summary>
        /// <code>
        /// POST /user/_id_/upd
        /// </code>
        public void upd(WebContext wc, string subscpt)
        {
            User obj = wc.JObj.ToObj<User>();
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("UPDATE users")._SET_(obj)._("WHERE id = @1");
                if (dc.Execute(sql.ToString(), p => { obj.Save(p); p.Put(subscpt); }) > 0)
                {
                    wc.StatusCode = 200; // ok
                }
                else
                {
                    wc.StatusCode = 406; // not acceptable
                }
            }
        }

    }
}