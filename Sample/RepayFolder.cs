using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class RepayFolder : WebFolder
    {
        public RepayFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<ItemVarFolder>();
        }

        #region /shop/-shopid-/item/

        public void lst(WebActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1 AND enabled", p => p.Set(shopid)))
                {
                    ac.Reply(200, dc.Dump<JsonContent>());
                }
                else
                {
                    ac.Reply(204);
                }
            }
        }

        #endregion

        #region /shop/-shopid-/order/

        [Shop]
        [Ui]
        public void @default(WebActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.ReplyGridPage(200, dc.ToList<Item>());
                }
                else
                {
                    ac.ReplyGridPage(200, (List<Item>)null);
                }
            }
        }

        [Shop]
        public void _cat_(WebActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.ReplyPane(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        #endregion
    }
}