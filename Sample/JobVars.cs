using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Shop;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    [Ui("设置"), User]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarVarWork, int>();

            Create<MyOrderWork>("order");
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            ac.GivePage(200, m =>
            {
                m.TOOLBAR();
                m.BOARDVIEW(h =>
                {
                    h.CAPTION("我的个人资料");
                    h.FIELD(prin.name, "姓名");
                    h.FIELD(prin.tel, "电话");
                    h.FIELD_("地址").T(prin.city)._T(prin.addr)._FIELD();
                    h.TAIL();
                });
            });
        }

        [Ui("身份刷新"), Tool(ButtonOpen)]
        public void token(ActionContext ac)
        {
            string wx = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const byte proj = 0xff ^ CREDENTIAL;
                if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(wx)))
                {
                    var o = dc.ToObject<User>(proj);
                    ac.SetTokenCookie(o, proj);
                    ac.GivePane(200);
                }
                else
                {
                    ac.GivePane(404);
                }
            }
        }
    }

    public class MyVarVarWork : Work
    {
        public MyVarVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string wx = ac[-1];
            var prin = (User) ac.Principal;
            if (ac.GET)
            {
                if (ac.Query.Count > 0)
                {
                    ac.Query.Let(out prin.name).Let(out prin.tel).Let(out prin.city);
                }
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.TEXT(nameof(prin.name), prin.name, label: "姓名", max: 4, min: 2, required: true);
                    h.TEXT(nameof(prin.tel), prin.tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true);
                    string city = prin.city ?? City.All[0].name;
                    h.SELECT(nameof(prin.city), city, City.All, "城市");
                    h.TEXT(nameof(prin.addr), prin.addr, label: "地址", max: 10, min: 2, required: true);
                    h._FORM();
                });
            }
            else
            {
                const byte proj = 0xff ^ CREDENTIAL ^ User.LATER;
                var o = await ac.ReadObjectAsync(obj: prin);
                o.wx = wx;
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("INSERT INTO users")._(o, proj)._VALUES_(o, proj).T(" ON CONFLICT (wx) DO UPDATE")._SET_(o, proj ^ WX);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.SetTokenCookie(o, 0xff ^ CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        const string PASS = "0z4R4pX7";

        [Ui("设密码"), Tool(ButtonShow)]
        public async Task pass(ActionContext ac)
        {
            User prin = (User) ac.Principal;
            string wx = ac[-1];
            string credential;
            string password = null;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    credential = (string) dc.Scalar("SELECT credential FROM users WHERE wx = @1", (p) => p.Set(wx));
                    if (credential != null)
                    {
                        password = PASS;
                    }
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("用于微信以外登录");
                        h.PASSWORD(nameof(password), password, label: "密码", max: 10, min: 3, required: true);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                password = f[nameof(password)];
                if (password != PASS)
                {
                    credential = StrUtility.MD5(prin.tel + ":" + password);
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET credential = @1 WHERE wx = @1", (p) => p.Set(credential).Set(wx));
                    }
                }
                ac.GivePane(200);
            }
        }
    }

    /// <summary>
    /// The working folder of shop operators.
    /// </summary>
    [Ui("常规"), User(OPR)]
    public class OprVarWork : Work, IShopVar
    {
        public OprVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<OprNewlyWork>("newly");

            Create<OprPastlyWork>("pastly");

            Create<OprCartWork>("cart");

            Create<OprItemWork>("item");

            Create<OprCashWork>("cash");
        }

        public void @default(ActionContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (!inner)
            {
                ac.GiveFrame(200, false, 60 * 15, "店铺操作");
                return;
            }

            var shops = Obtain<Map<string, Shop>>();
            string shopid = ac[this];
            ac.GivePage(200, h =>
            {
                h.TOOLBAR();
                var o = shops[shopid];
                h.BOARDVIEW_();

                h.CARD_();
                h.CAPTION(o.name, Statuses[o.status], o.status == 2);
                h.FIELD(o.schedule, "时间");
                h.FIELD_("派送").T(o.delivery);
                if (o.areas != null) h._T("限送").T(o.areas);
                h._FIELD();
                h.FIELD_("计价").T(o.min).T("元起订，每满").T(o.notch).T("元立减").T(o.off).T("元")._FIELD();
                h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                h.FIELD_("客服").T(o.oprname)._T(o.oprtel)._FIELD();
                h.TAIL();
                h._CARD();

                h._BOARDVIEW();
            });
        }

        [Ui("人员"), Tool(ButtonOpen, 2), User(OPRMGR)]
        public async Task acl(ActionContext ac, int cmd)
        {
            string shopid = ac[this];
            string wx;
            string tel = null;
            short opr = 0;
            if (cmd == 1) // remove
            {
                var f = await ac.ReadAsync<Form>();
                wx = f[nameof(wx)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE users SET opr = 0, oprat = NULL WHERE wx = @1", p => p.Set(wx));
                }
            }
            else if (cmd == 2) // add
            {
                var f = await ac.ReadAsync<Form>();
                tel = f[nameof(tel)];
                opr = f[nameof(opr)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE users SET opr = @1, oprat = @2 WHERE tel = @3", p => p.Set(opr).Set(shopid).Set(tel)); // may add multiple
                }
            }
            ac.GivePane(200, m =>
            {
                m.FORM_();
                m.FIELDSET_("现有人员");
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT wx, name, tel, opr FROM users WHERE oprat = @1", p => p.Set(shopid)))
                    {
                        m.T("<div>");
                        while (dc.Next())
                        {
                            dc.Let(out wx).Let(out string uname).Let(out string utel).Let(out short uopr);
                            m.RADIO(nameof(wx), wx, label: utel + ' ' + uname + ' ' + Oprs[uopr]);
                        }
                        m.T("</div>");
                        m.BUTTON(nameof(acl), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加人员");
                m.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, box: 8).SELECT(nameof(opr), opr, Oprs, box: 4);
                m.BUTTON(nameof(acl), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }

        [Ui("设置"), Tool(ButtonShow, 2), User(OPRMGR)]
        public async Task sets(ActionContext ac)
        {
            var shops = Obtain<Map<string, Shop>>();
            string shopid = ac[this];
            var o = shops[shopid];
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.TEXT(nameof(o.schedule), o.schedule, "时间");
                    h.TEXT(nameof(o.delivery), o.delivery, "派送");
                    h.TEXTAREA(nameof(o.areas), o.areas, "限送");
                    h.NUMBER(nameof(o.min), o.min, "起订", box: 4).NUMBER(nameof(o.notch), o.notch, "满额", box: 4).NUMBER(nameof(o.off), o.off, "扣减", box: 4);
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                o.schedule = f[nameof(o.schedule)];
                o.delivery = f[nameof(o.delivery)];
                string v = f[nameof(o.areas)];
                o.areas = v.Split('\n');
                o.min = f[nameof(o.min)];
                o.notch = f[nameof(o.notch)];
                o.off = f[nameof(o.off)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET schedule = @1, delivery = @2, areas = @3, min = @4, notch = @5, off = @6 WHERE id = @7",
                        p => p.Set(o.schedule).Set(o.delivery).Set(o.areas).Set(o.min).Set(o.notch).Set(o.off).Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("图示"), Tool(ButtonCrop, Ordinals = 4), User(OPRMGR)]
        public async Task img(ActionContext ac, int ordinal)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT img" + ordinal + " FROM shops WHERE shopid = @1", p => p.Set(shopid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else ac.Give(200, new StaticContent(byteas));
                    }
                    else ac.Give(404); // not found
                }
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                ArraySegment<byte> jpeg = f[nameof(jpeg)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET img" + ordinal + " = @1 WHERE id = @2", p => p.Set(jpeg).Set(shopid));
                }
                ac.Give(200); // ok
            }
        }

        [Ui("上下班"), Tool(ButtonShow), User(OPRSTAFF)]
        public async Task status(ActionContext ac)
        {
            var shops = Obtain<Map<string, Shop>>();

            User prin = (User) ac.Principal;
            string shopid = ac[this];
            var o = shops[shopid];
            bool custsvc;
            if (ac.GET)
            {
                custsvc = o.oprwx == prin.wx;
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("设置网点营业状态").SELECT(nameof(o.status), o.status, Statuses, "营业状态")._FIELDSET();
                    h.FIELDSET_("客服设置");
                    h.CHECKBOX(nameof(custsvc), custsvc, "我要作为上线客服");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                o.status = f[nameof(o.status)];
                custsvc = f[nameof(custsvc)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET status = @1 WHERE id = @2", p => p.Set(o.status).Set(shopid));
                    if (custsvc)
                    {
                        dc.Execute("UPDATE shops SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(shopid));
                        o.oprwx = prin.wx;
                        o.oprtel = prin.tel;
                        o.oprname = prin.name;
                    }
                    else
                    {
                        dc.Execute("UPDATE shops SET oprwx = NULL, oprtel = NULL, oprname = NULL WHERE id = @1", p => p.Set(shopid));
                        o.oprwx = null;
                        o.oprtel = null;
                        o.oprname = null;
                    }
                }
                ac.GivePane(200);
            }
        }
    }
}