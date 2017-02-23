using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class ItemFolder : Folder
    {
        public ItemFolder(FolderContext fc) : base(fc)
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
                    ac.Reply(204); // no content
                }
            }
        }


        // [Shop]
        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.ReplyFolderPage(200, dc.ToList<Item>());
                }
                else
                {
                    ac.ReplyFolderPage(200, (List<Item>)null);
                }
            }
        }

        // [Shop]
        [Ui("新建", 3)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                Item o = Item.Empty;
                ac.ReplyForm(200, o);
            }
            else // post
            {
                var item = await ac.ReadObjectAsync<Item>();
                using (var dc = Service.NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty)._VALUES_(Item.Empty)._("");
                    if (dc.Execute(p => p.Set(item)) > 0)
                    {
                        ac.Reply(201); // created
                    }
                    else
                    {
                        ac.Reply(500); // internal server error
                    }
                }
            }
        }

        [Shop]
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

        [Shop]
        [Ui]
        public void toggle(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.Reply(303); // see other
            }
        }

        [Shop]
        [Ui]
        public async Task modify(ActionContext ac)
        {
            string shopid = ac[1];

            if (ac.GET)
            {
                var item = new Item() { };
                ac.ReplyForm(200, item);
            }
            else
            {
                var item = await ac.ReadObjectAsync<Item>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                    // ac.SetHeader();
                    ac.Reply(303); // see other
                }
            }
        }
    }
}