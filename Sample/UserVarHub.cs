using System.Net;
using System.Text;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The features about a particular user principal.</summary>
    /// /-/
    ///
    public class UserVarHub : WebVarHub
    {
        public UserVarHub(WebConfig cfg) : base(cfg)
        {
        }

        ///
        /// Gets a token
        ///
        public override void Default(WebContext wc, string id)
        {
            string password;
            if (wc.GetParam("password", out password))
            {
                wc.Response.StatusCode = 400;
                return;
            }
            using (var dc = Service.NewSqlContext())
            {
                if (dc.QueryA("SELECT id, credential, name FROM users WHERE id = @id", (p) => p.Set("@id", id)))
                {
                    User o = new User();
                    dc.Get(out o.id);
                    dc.Get(out o.credential);
                    dc.Get(out o.name);

                    string md5 = ComputeMD5(password);
                    if (md5.Equals(o.credential))
                    {
                        wc.StatusCode = 200;
                        wc.SetSerialObj(o);
                    }
                    else
                    {
                        wc.Response.StatusCode = 400;
                    }
                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        [ToSelf]
        public void ChPwd(WebContext wc, string userid)
        {
            ISerialReader r = wc.Serial;

            int ret = 0;

            r.ReadArray(() =>
            {
                r.Read(out ret);
            });

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Execute("UPDATE users SET password = @id WHERE id = @id", (p) => p.Set("@id", userid)) > 0)
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                }
            }
            //            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, string x)
        {
            //            wc.Response.SendFileAsync()
        }

        internal static string ComputeMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}