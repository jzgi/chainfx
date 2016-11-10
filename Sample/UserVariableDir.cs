using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The user variable-hub controller.
    ///
    public class UserVariableDir : WebDir, IVariable
    {
        public UserVariableDir(WebDirContext wnc) : base(wnc)
        {
        }

        ///
        /// Get a user token.
        ///
        /// <code>
        /// GET /user/_id_/?password=_password_
        /// </code>
        public void @default(WebContext wc, string sub)
        {
            string id = wc.GetVar(this);
            string password = null;
            if (!wc.Get(nameof(password), ref password))
            {
                wc.StatusCode = 400; return;
            }
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM users WHERE id = @1", (p) => p.Put(id)))
                {
                    Token obj = dc.ToData<Token>(0xff);
                    string credential = StrUtility.MD5(id + ':' + ':' + password);
                    if (credential.Equals(obj.credential))
                    {
                        JsonContent cont = new JsonContent(true, false, 256);
                        cont.PutObj(obj);
                        cont.Encrypt(0x4a78be76, 0x1f0335e2);
                        wc.Send(200, cont);
                    }
                    else
                        wc.StatusCode = 400;
                }
                else
                    wc.StatusCode = 404;
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
            User m = wc.ReadData<User>();
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("UPDATE users")._SET_(m)._("WHERE id = @1");
                if (dc.Execute(sql.ToString(), p => { m.Dump(p); p.Put(subscpt); }) > 0)
                {
                    wc.StatusCode = 200; // ok
                }
                else
                    wc.StatusCode = 406; // not acceptable
            }
        }

    }
}