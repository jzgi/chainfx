using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// <summary>The user directory service</summary>
	///
	public class UserService : WebService
	{
		// user cache
		// MemoryCache cache = new MemoryCache(null);

		public UserService(WebServiceBuilder builder) : base(builder)
		{
			AddSub<UserAdminSub>("admin", (x) => x.Can(null, 1));

			MountHub<UserZoneHub, User>((x, p) => true);
		}

		///
		/// Registers or creates a user account.
		///
		public void Register(WebContext wc)
		{
		}

		public void Search(WebContext wc)
		{
		}

		//
		// EVENT HANDLING
		//

		public void OnBizEnroll()
		{
		}
	}
}