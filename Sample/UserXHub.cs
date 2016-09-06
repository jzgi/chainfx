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
            using (var sc = Service.NewSqlContext())
            {
                if (sc.QueryOne("SELECT * FROM users WHERE id = @id", (p) => p.Add("@id", id)))
                {
//                    User u = sc.ReadRow<User>();

//
                    User u = new User();
                    sc.Read(ref u.id);
                    sc.Read(ref u.name);

                    wc.Response.SetObject(u);
                }
            }
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        public void Modify(WebContext wc, string x)
        {
//            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, string x)
        {
//            wc.Response.SendFileAsync()
        }
    }
}