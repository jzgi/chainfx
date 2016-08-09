using Greatbone.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Greatbone.Sample
{
	///
	/// The user directory service controller.
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