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
                if (dc.QueryA("SELECT * FROM users WHERE id = @1", (p) => p.Put(id)))
                {
                    Token obj = new Token();
                    obj.Load(dc, Token.Out);
                    string c16 = StringUtility.C16(password);
                    if (c16.Equals(obj.credential))
                    {
                        wc.Respond(200, obj, Token.Out);
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
            JObj jo = wc.JObj;

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