using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary></summary>
	/// /-/
	///
    public class UserUnitHub : WebUnitHub<User>
    {
        public UserUnitHub(WebServiceBuilder builder) : base(builder)
        {
        }

        ///
        /// Gets a token
        ///
        public override void Default(WebContext wc, User unit)
        {
//            wc.Response.SendFileAsync()
        }

        /// <summary>To modify the user's profile, normally by him/her self.</summary>
        ///
        public void Modify(WebContext wc, User unit)
        {
//            wc.Response.SendFileAsync()
        }

        ///
        /// The user drops this account
        ///
        public void Drop(WebContext wc, User unit)
        {
//            wc.Response.SendFileAsync()
        }
    }
}