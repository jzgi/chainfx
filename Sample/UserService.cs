using Greatbone.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Greatbone.Sample
{
    ///
    ///  The service controller for user-related functions.
    ///   /user/-123-/
    ///   /user/
    ///
    public class UserService : WebService
    {
        // user cache
        // MemoryCache cache = new MemoryCache(null);

        public UserService(WebServiceContext wsc) : base(wsc)
        {
            AddSub<DirectoryAdminSub>("admin", (x) => x.Can(null, 1));

            MountHub<DirectoryUserHub, User>((x, p) => true);
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