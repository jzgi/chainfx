using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public abstract class TipoffWork<V> : Work where V : TipoffVarWork
    {
        protected TipoffWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, int>(obj => ((Tipoff) obj).id);
        }
    }

    [Ui("举报")]
    [User]
    public class MyTipoffWork : TipoffWork<MyTipoffVarWork>
    {
        public MyTipoffWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM tipoffs WHERE wx = @1 ORDER BY id DESC", p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Tipoff>(-1));
                }
                else
                {
                    ac.GiveGridPage(200, (Tipoff[]) null);
                }
            }
        }
    }

    [Ui("举报管理")]
    [User(spr: true)]
    public class SprTipoffWork : TipoffWork<SprTipoffVarWork>
    {
        public SprTipoffWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Item.ICON;
                dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                if (dc.Query(p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Item>(proj), proj ^ Item.SHOPID);
                }
                else
                {
                    ac.GiveGridPage(200, (Item[]) null);
                }
            }
        }
    }
}