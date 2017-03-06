using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("付款管理")]
    public class PayFolder : Folder
    {
        public PayFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ItemVarFolder>();
        }

        public void lst(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1 AND enabled", p => p.Set(shopid)))
                {
                    ac.Reply(200, dc.Dump());
                }
                else
                {
                    ac.Reply(204);
                }
            }
        }

        [User]
        [Ui]
        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.ReplyFolderPage(200, dc.ToList<Item>());
                }
                else
                {
                    ac.ReplyFolderPage(200, (List<Item>)null);
                }
            }
        }

        [User]
        public void _cat_(ActionContext ac)
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
    }
}