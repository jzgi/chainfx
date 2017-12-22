using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Shop;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    [Ui("常规"), User]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyCartWork>("cart");

            Create<MyOrderWork>("order");
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            ac.GivePage(200, m =>
            {
                m.TOOLBAR();
                m.BOARDVIEW(
                    h =>
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
                const short proj = -1 ^ CREDENTIAL;
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

        [Ui("修改", Group = 1), Tool(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string wx = ac[this];
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
                    h.SELECT(nameof(prin.city), city, City.All, "城市", refresh: true);

                    h.TEXT(nameof(prin.addr), prin.addr, label: "场址", max: 10, min: 2, required: true);
                    h._FORM();
                });
            }
            else
            {
                const short proj = -1 ^ CREDENTIAL ^ User.LATER;
                var o = await ac.ReadObjectAsync(obj: prin);
                o.wx = wx;
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("INSERT INTO users")._(o, proj)._VALUES_(o, proj).T(" ON CONFLICT (wx) DO UPDATE")._SET_(o, proj ^ WX);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.SetTokenCookie(o, -1 ^ CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        const string PASS = "0z4R4pX7";

        [Ui("设密码", Group = 1), Tool(ButtonShow)]
        public async Task pass(ActionContext ac)
        {
            User prin = (User) ac.Principal;
            string wx = ac[this];
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
                return;
            }

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
                ac.GiveFrame(200, false, 60 * 15, "粗粮达人网点操作");
                return;
            }

            string shopid = ac[this];
            ac.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                    dc.Query1(p => p.Set(shopid));
                    var o = dc.ToObject<Shop>();
                    h.BOARDVIEW_();

                    h.CARD_();
                    h.CAPTION(o.name, Statuses[o.status], o.status == 2);
                    h.FIELD(o.schedule, "时间");
                    h.FIELD_("派送").T(o.delivery);
                    if (o.areas != null) h._T("限送").T(o.areas);
                    h._FIELD();
                    h.FIELD_("计价").T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元")._FIELD();
                    h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                    h.FIELD_("客服").T(o.oprname)._T(o.oprtel)._FIELD();
                    h.TAIL();
                    h._CARD();

                    h.CARD_();
                    h.CAPTION("盘存");
                    if (o.articles != null)
                    {
                        for (int i = 0; i < o.articles.Length; i++)
                        {
                            var sup = o.articles[i];
                            h.FIELD(sup.name, box: 8).FIELD(sup.qty, box: 2).FIELD(sup.unit, box: 2);
                        }
                    }
                    h.TAIL();
                    h._CARD();

                    h._BOARDVIEW();
                }
            });
        }

        [Ui("人员"), Tool(ButtonOpen, 2), User(OPRMGR)]
        public async Task access(ActionContext ac, int cmd)
        {
            string shopid = ac[this];
            string tel = null;
            short opr = 0;
            var f = await ac.ReadAsync<Form>();
            if (f != null)
            {
                tel = f[nameof(tel)];
                opr = f[nameof(opr)];
                if (cmd == 1) // remove
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET opr = 0, oprat = NULL WHERE tel = @1", p => p.Set(tel));
                    }
                }
                else if (cmd == 2) // add
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET opr = @1, oprat = @2 WHERE tel = @3", p => p.Set(opr).Set(shopid).Set(tel));
                    }
                }
            }
            ac.GivePane(200, m =>
            {
                m.FORM_();
                m.FIELDSET_("现有人员");
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT name, tel, opr FROM users WHERE oprat = @1", p => p.Set(shopid)))
                    {
                        m.T("<div>");
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out tel).Let(out opr);
                            m.RADIO(nameof(tel), tel, label: tel + ' ' + name + ' ' + Oprs[opr]);
                        }
                        m.T("</div>");
                        m.BUTTON(nameof(access), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加人员");
                m.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, box: 8).SELECT(nameof(opr), opr, Oprs, box: 4);
                m.BUTTON(nameof(access), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }

        [Ui("设置"), Tool(ButtonShow, 2), User(OPRMGR)]
        public async Task sets(ActionContext ac)
        {
            string shopid = ac[this];
            string schedule;
            string delivery;
            string[] areas;
            decimal min, notch, off;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT city, schedule, delivery, areas, min, notch, off FROM shops WHERE id = @1", p => p.Set(shopid));
                    dc.Let(out string city).Let(out schedule).Let(out delivery).Let(out areas).Let(out min).Let(out notch).Let(out off);
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.TEXT(nameof(schedule), schedule, "时间");
                        h.TEXT(nameof(delivery), delivery, "派送");
                        h.SELECT(nameof(areas), areas, City.AreasOf(city), "限送");
                        h.NUMBER(nameof(min), min, "起送", box: 4).NUMBER(nameof(notch), notch, "满额", box: 4).NUMBER(nameof(off), off, "扣减", box: 4);
                        h._FORM();
                    });
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();
            f.Let(out schedule).Let(out delivery).Let(out areas).Let(out min).Let(out notch).Let(out off);
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE shops SET schedule = @1, delivery = @2, areas = @3, min = @4, notch = @5, off = @6 WHERE id = @7",
                    p => p.Set(schedule).Set(delivery).Set(areas).Set(min).Set(notch).Set(off).Set(shopid));
            }
            ac.GivePane(200);
        }

        [Ui("图示"), Tool(ButtonCrop, Ordinals = 4), User(OPRMEM)]
        public async Task img(ActionContext ac, int ordinal)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT img" + ordinal + " FROM shops WHERE id = @1", p => p.Set(shopid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                    }
                    else ac.Give(404, @public: true, maxage: 60 * 5); // not found
                }
                return;
            }
            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE shops SET img" + ordinal + " = @1 WHERE id = @2", p => p.Set(jpeg).Set(shopid));
            }
            ac.Give(200); // ok
        }

        [Ui("上下班", Group = 1), Tool(ButtonShow), User(OPRMEM)]
        public async Task status(ActionContext ac)
        {
            User prin = (User) ac.Principal;
            string shopid = ac[this];
            short status;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        status = (short) dc.Scalar("SELECT status FROM shops WHERE id = @1", p => p.Set(shopid));
                        h.FIELDSET_("设置网点营业状态").SELECT(nameof(status), status, Statuses, "营业状态")._FIELDSET();

                        bool on = dc.Query1("SELECT 1 FROM shops WHERE id = @1 AND oprwx = @2", p => p.Set(shopid).Set(prin.wx));
                        string hint = on ? "我确定客服下线。这将会置空客服电话号码，也不接收客服通知" : "我确定客服上线，个人电话号码显示为客服电话，并且接收客服通知";
                        const bool yes = false;
                        h.FIELDSET_(on ? "我客服下线" : "我客服上线");
                        h.CHECKBOX(nameof(yes), yes, hint, required: true);
                        h._FIELDSET();
                    }
                    h._FORM();
                });
                return;
            }

            var f = await ac.ReadAsync<Form>();
            status = f[nameof(status)];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE shops SET status = @1 WHERE id = @2", p => p.Set(status).Set(shopid));
                if (dc.Query1("SELECT 1 FROM shops WHERE id = @1 AND oprwx = @2", p => p.Set(shopid).Set(prin.wx)))
                {
                    dc.Execute("UPDATE shops SET oprwx = NULL, oprtel = NULL, oprname = NULL WHERE id = @1", p => p.Set(shopid));
                }
                else
                {
                    dc.Execute("UPDATE shops SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(shopid));
                }
            }
            ac.GivePane(200);
        }

        [Ui("调整", Group = 2), Tool(ButtonShow, 2), User(OPRMEM)]
        public async Task adjust(ActionContext ac)
        {
            string shopid = ac[this];
            short status;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query1("SELECT articles FROM shops WHERE id = @1", p => p.Set(shopid));
                        dc.Let(out Article[] articles);
                        if (articles != null)
                        {
                            for (int i = 0; i < articles.Length; i++)
                            {
                                var o = articles[i];
                                h.FIELD(o.name, box: 5).NUMBER(o.name, (short) 0, min: (short) 0, step: (short) 1, box: 5).INPBUTTON("X", "$(this).closest()");
                            }
                        }
                        h.INPBUTTON("+", "");
                    }
                    h._FORM();
                });
            }
        }
    }
}