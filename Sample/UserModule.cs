using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The user directory service</summary>
    ///
    public class UserModule : WebModule, IAdmin
    {
        public UserModule(ISetting setg) : base(setg)
        {
            SetVarHub<UserVarHub>(false);
        }

        ///
        /// Creates a new user account.
        ///
        public void @new(WebContext wc)
        {
            User u = null;

            using (var sc = Service.NewDbContext())
            {
                sc.Execute("INSERT INTO users () VALUES (@id, @credential, @name, @fame, @brand, @loggedin)",
                    p => { p.Put("@id", ""); }
                );

                // wc.SetContent(u);
            }
        }

        //
        // ADMIN
        //

        public void search(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.Bytes;
        }

        public void del(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc)
        {
            throw new NotImplementedException();
        }

    }
}