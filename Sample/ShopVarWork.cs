using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class ShopVarWork : Work
    {
        protected ShopVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class HublyShopVarWork : TeamVarWork
    {
        public HublyShopVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui(icon: "file-edit"), Tool(ButtonShow)]
        public async Task edit(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[this];
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Team.Empty, 0).T(" FROM orgs WHERE id = @1");
                    var o = dc.Query1<Team>(p => p.Set(orgid), 0);
                    wc.GivePane(200, h =>
                    {
                        h.FORM_().FIELDUL_("填写网点资料");
                        h.LI_().TEXT("名　称", nameof(o.name), o.name, max: 10, required: true)._LI();
                        h.LI_().TEXT("地　址", nameof(o.addr), o.addr, max: 20)._LI();
                        h.LI_().NUMBER("经　度", nameof(o.x), o.x, step: 0.000001).NUMBER("纬　度", nameof(o.y), o.y, step: 0.000001)._LI();
                        h.LI_().SELECT("状　态", nameof(o.status), o.status, Team.Statuses, required: true)._LI();
                        h._FIELDUL()._FORM();
                    });
                }
            }
            else // post
            {
                var o = await wc.ReadObjectAsync<Team>(0);
                o.hubid = hubid;
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orgs")._SET_(Team.Empty, 0).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, 0);
                        p.Set(orgid);
                    });
                }
                wc.GivePane(200);
            }
        }

        [Ui(icon: "user", tip: "设置负责人"), Tool(ButtonShow)]
        public async Task mgr(WebContext wc)
        {
            string orgid = wc[this];
            string tel_name_wx;
            if (wc.IsGet)
            {
                string forid = wc.Query[nameof(forid)];
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDUL_("查询帐号（手机号）");
                    m.SEARCH(null, nameof(forid), forid, pattern: "[0-9]+", max: 11, min: 11);
                    m._FIELDUL();
                    if (forid != null)
                    {
                        using (var dc = NewDbContext())
                        {
                            if (dc.Query1("SELECT tel, name, wx, teamat, teamly FROM users WHERE tel = @1", p => p.Set(forid)))
                            {
                                dc.Let(out string tel).Let(out string name).Let(out string wx).Let(out string teamat).Let(out string teamly);
                                m.FIELDUL_("设置");
                                m.RADIO(nameof(tel_name_wx), tel + " " + name + " " + wx, tel + " " + name);
                                m._FIELDUL();
                            }
                        }
                    }
                    m._FORM();
                });
            }
            else // post
            {
                var f = await wc.ReadAsync<Form>();
                tel_name_wx = f[nameof(tel_name_wx)];
                (string wx, string tel, string name) = tel_name_wx.ToTriple();
                using (var dc = NewDbContext())
                {
                    dc.Execute(@"UPDATE orgs SET mgrtel = @1, mgrname = @2, mgrwx = @3 WHERE id = @4; 
                        UPDATE users SET teamly = 7, teamat = @4 WHERE wx = @1;", p => p.Set(wx).Set(tel).Set(name).Set(orgid));
                }
                wc.GivePane(200);
            }
        }
    }
}