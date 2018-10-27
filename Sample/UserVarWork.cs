using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class TeamUserVarWork : UserVarWork
    {
        public TeamUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        
        [Ui(icon: "list", tip: "个人订单"), Tool(AnchorOpen)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short uid = wc[this];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE uid = @1 ORDER BY status DESC", p => p.Set(uid));
                wc.GivePage(200, h =>
                {
                    h.BOARD(arr, o =>
                        {
                            h.HEADER_("uk-card-header");
                            h.T("收货：").T(o.uaddr).SP().T(o.uname).SP().T(o.utel);
                            h._HEADER();
                            h.MAIN_("uk-card-body uk-row");
                            h.PIC_(css: "uk-width-1-6").T("/").T(hubid).T("/").T(o.itemid).T("/icon")._PIC();
                            h.DIV_("uk-width-2-3").SP().T(o.item).SP().CNY(o.price).T(o.qty).T("/").T(o.unit)._DIV();
                            h.VARTOOLS(css: "uk-width-1-6");
                            h._MAIN();
                        }
                    );
                }, false, 3, title: "我的订单", refresh: 120);
            }
        }

        [Ui(icon: "file-edit", tip: "修改该记录"), Tool(ButtonShow)]
        public async Task upd(WebContext wc)
        {
            string hubid = wc[0];
            short uid = wc[this];
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<User>("SELECT * FROM users WHERE id = @1", p => p.Set(uid));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_().FIELDUL_("填写客户资料");
                        h.LI_().TEXT("客户名称", nameof(o.name), o.name, tip: "姓名或昵称", max: 4, min: 2, required: true)._LI();
                        h.LI_().TEXT("手　　机", nameof(o.tel), o.tel, pattern: "[0-9]+", tip: "个人手机号", max: 11, min: 11, required: true)._LI();
                        h.LI_().TEXT("收货地址", nameof(o.addr), o.addr, max: 30)._LI();
                        h._FIELDUL()._FORM();
                    });
                }
            }
            else
            {
                var o = await wc.ReadObjectAsync(0, obj: new User
                {
                    hubid = hubid,
                    teamid = uid
                });
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE users")._SET_(User.Empty, 0).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, 0);
                        p.Set(uid);
                    });
                }
                wc.GivePane(200);
            }
        }
    }

    public class ShopUserVarWork : UserVarWork
    {
        public ShopUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class HublyOprVarWork : UserVarWork
    {
        public HublyOprVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}