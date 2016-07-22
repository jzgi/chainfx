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
    public class DirService : WebService
    {
        // user cache
        // MemoryCache cache = new MemoryCache(null);

        public DirService(WebCreationContext wcc) : base(wcc)
        {
            AddSub<DirAdminSub>("admin", (x) => x.Can(null, 1));

            SetMux<DirUserMux, User>((x, p) => true);
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