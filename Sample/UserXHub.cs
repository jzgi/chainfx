using System.Net;
using System.Text;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The features about a particular user principal.</summary>
    /// /-/
    ///
    public class UserXHub : WebXHub
    {
        public UserXHub(WebServiceContext wsc) : base(wsc)
        {
        }

        ///
        /// Gets a token
        ///
        public override void Default(WebContext wc, string id)
        {
            string password = null;
            wc.Request.GetParameter("password", ref password);
            using (var dc = Service.NewSqlContext())
            {
                if (dc.QueryA("SELECT id, credential, name FROM users WHERE id = @id", (p) => p.Set("@id", id)))
                {
                    User o = new User();
                    dc.Get(ref o.id);
                    dc.Get(ref o.credential);
                    dc.Get(ref o.name);

                    string md5 = ComputeMD5(password);
                    if (md5.Equals(o.credential))
                    {
                        wc.Response.StatusCode = (int)HttpStatusCode.OK;
                        wc.Response.SetContentAsJson(o);
                    }
                    else
                    {
                        wc.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        [Self]
        public void ChPwd(WebContext wc, string userid)
        {
            ISerialReader r = wc.Request.Reader;

            int ret = 0;

            r.ReadArray(() =>
            {
                r.Read(ref ret);
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