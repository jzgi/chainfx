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
    public class DirectoryService : WebService
    {
        // user cache
        // MemoryCache cache = new MemoryCache(null);

        public DirectoryService(WebCreationContext wcc) : base(wcc)
        {
            AddSub<DirectoryAdminSub>("admin", (x) => x.Can(null, 1));

            SetMux<DirectoryUserMux, User>((x, p) => true);
        }


        public override void Default(WebContext wc)
        {
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
    }
}