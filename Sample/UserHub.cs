using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The user directory service</summary>
    ///
    public class UserHub : WebHub
    {
        public UserHub(WebConfig cfg) : base(cfg)
        {
            SetVarHub<UserVarHub>(false);
        }

        ///
        /// Creates a new user account.
        ///
        public void New(WebContext wc)
        {
            User u = null;

            using (var sc = Service.NewSqlContext())
            {
                sc.Execute("INSERT INTO users () VALUES (@id, @credential, @name, @fame, @brand, @loggedin)",
                    p => { p.Set("@id", ""); }
                );

                // wc.SetContent(u);
            }
        }

        public void Find(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.Bytes;
            ad = wc;
        }

        WebContext ad;
    }
}