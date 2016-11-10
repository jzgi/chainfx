using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all service controllers.
    /// </summary>
    ///
    public abstract class AbstServiceWork : WebServiceWork
    {
        readonly Login[] logins;

        public AbstServiceWork(WebConfig cfg) : base(cfg)
        {
            logins = JsonUtility.FileToDatas<Login>(cfg.GetFilePath("$realm.json"));
        }


        protected override IPrincipal GetPrincipal(bool token, string idstr)
        {
            if (token) // token
            {
                string plain = StrUtility.Decrypt(idstr, 0x4a78be76, 0x1f0335e2); // plain token
                JsonParse par = new JsonParse(plain);
                try
                {
                    Obj jo = (Obj)par.Parse();
                    return jo.ToData<Token>();
                }
                catch { }
            }
            else // username
            {
                if (logins != null)
                {
                    for (int i = 0; i < logins.Length; i++)
                    {
                        Login lgn = logins[i];
                        if (lgn.id.Equals(idstr)) return lgn;
                    }
                }
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
                        WebWork child = Children[i];
                    }
                },
                true);
            }
        }

    }

}