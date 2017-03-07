using Greatbone.Core;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Greatbone.Sample
{
    [Ui("购物车管理")]
    public class CartFolder : Folder
    {
        // all carts keyed by userid
        readonly ConcurrentDictionary<string, Cart> carts;

        public CartFolder(FolderContext dc) : base(dc)
        {
            CreateVar<CartVarFolder>(tok => ((User)tok).id);

            carts = new ConcurrentDictionary<string, Cart>(8, 1024);
        }

        public ConcurrentDictionary<string, Cart> Carts => carts;

        public void @default(ActionContext ac)
        {
            ac.GiveFolderPage(200, (List<Item>)null);
        }
    }
}