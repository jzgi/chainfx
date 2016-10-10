using System.Net;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The features for a particular user account.</summary>
    ///
    public class UserVarHub : WebVarHub
    {
        public UserVarHub(ISetting setg) : base(setg)
        {
        }

        ///
        /// GET /user/_id_/?password=_password_
        ///
        public override void @default(WebContext wc, string id)
        {
            string password = null;
            if (!wc.Got(nameof(password), ref password))
            {
                wc.StatusCode = 400; return;
            }
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT id, credential, name FROM users WHERE id = @1", (p) => p.Put(id)))
                {
                    User o = new User();
                    dc.Got(ref o.id);
                    dc.Got(ref o.credential);
                    dc.Got(ref o.name);

                    string c16 = StrUtility.C16(password);
                    if (c16.Equals(o.credential))
                    {
                        wc.SendJson(200, jcont => jcont.PutObj(o));
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

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        [IfSelf]
        public void chpwd(WebContext wc, string userid)
        {
            JObj r = (JObj)wc.Data;

            int ret = 0;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE users SET password = @id WHERE id = @id", (p) => p.Put("@id", userid)) > 0)
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                }
            }
        }

    }
}