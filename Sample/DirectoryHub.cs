using Greatbone.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Greatbone.Sample
{
  ///
  ///  The hub controller for user-related functions.
  ///   /user/-123-/
  ///   /user/
  ///
  public class DirectoryHub : WebHub
  {
//        MemoryCache cache = new MemoryCache("");

    public DirectoryHub(WebHub parent) : base(parent)
    {
      AddSub<DirectoryAdminSub>("admin", (x) => x.Can(null, 1));

      SetMux<DirectoryUserMux, User>((x, p) => true);
    }

    public override void Default(WebContext wc)
    {
    }

    public void Create(WebContext wc)
    {
    }

    public void Search(WebContext wc)
    {
    }
  }
}