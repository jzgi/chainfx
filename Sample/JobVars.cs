using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;
using static Greatbone.Sample.Shop;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    [Ui("常规"), Allow]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkContext wc) : base(wc)
        {
            Create<MyCartWork>("cart");

            Create<MyOrderWork>("order");

            CreateVar<MyVarVarWork, int>(); // dataless
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            var prin = (User) ac.Principal;
            ac.GivePage(200, m =>
            {
                m.TOOLBAR();
                m.GRIDVIEW(
                    h =>
                    {
                        h.CAPTION("我的个人资料");
                        h.FIELD(prin.name, "姓名", box: 12);
                        h.FIELD(prin.tel, "电话", box: 12);
                        h.FIELD_("地址", box: 12).T(prin.city)._T(prin.area)._T(prin.addr)._FIELD();
                    });
            });
        }

        [Ui("刷新"), Style(ButtonOpen)]
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
    }

    public class MyVarVarWork : Work
    {
        public MyVarVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改"), Style(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string wx = ac[-1];
            var prin = (User) ac.Principal;
            if (ac.GET)
            {
                if (ac.Query.Count > 0)
                {
                    ac.Query.Let(out prin.name).Let(out prin.tel).Let(out prin.city).Let(out prin.area);
                }

                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.TEXT(nameof(prin.name), prin.name, label: "姓名", max: 4, min: 2, required: true, box: 12);
                    h.TEXT(nameof(prin.tel), prin.tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true, box: 12);

                    string city = prin.city ?? City.All[0].Key;
                    h.SELECT(nameof(prin.city), city, City.All, "城市", refresh: true, box: 12);

                    var areas = City.All[city].Areas;
                    h.SELECT(nameof(prin.area), prin.area ?? areas[0].name, areas, "区域", box: 12);

                    h.TEXT(nameof(prin.addr), prin.addr, label: "场址", max: 10, min: 2, required: true, box: 12);
                    h._FORM();
                });
            }
            else
            {
                const short proj = -1 ^ CREDENTIAL ^ LATER;
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

        [Ui("设密码"), Style(ButtonShow)]
        public async Task pass(ActionContext ac)
        {
            User prin = (User) ac.Principal;
            string wx = ac[-1];
            string credential = null;
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
                        h.FIELDSET_("用于微信以外登录", box: 12);
                        h.PASSWORD(nameof(password), password, label: "密码", max: 10, min: 3, required: true, box: 12);
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


    [Ui("常规"), Allow(OPR)]
    public class OprVarWork : Work
    {
        public OprVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<OprVarVarWork, int>();

            Create<OprNewWork>("new");

            Create<OprGoWork>("go");

            Create<OprPastWork>("past");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (inner)
            {
                string shopid = ac[this];
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.GRIDVIEW(h =>
                    {
                        using (var dc = ac.NewDbContext())
                        {
                            dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                            dc.Query1(p => p.Set(shopid));
                            var o = dc.ToObject<Shop>();
                            h.CAPTION(o.name, Status[o.status], o.status == ON);
                            h.FIELDSET_("设置", box: 12);
                            h.FIELD(o.schedule, "时间", box: 12);
                            if (o.areas != null) h.FIELD(o.areas, "送达", box: 12);
                            h.FIELD_("计价", box: 12).T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元")._FIELD();
                            h._FIELDSET();
                            h.FIELDSET_("经理", box: 12);
                            h.FIELD(o.mgrname, "姓名", box: 12);
                            h.FIELD(o.mgrwx, "微信", box: 12);
                            h.FIELD(o.mgrtel, "电话", box: 12);
                            h._FIELDSET();
                            h.FIELDSET_("当前客服", box: 12);
                            h.FIELD(o.oprname, "姓名", box: 12);
                            h.FIELD(o.oprwx, "微信", box: 12);
                            h.FIELD(o.oprtel, "电话", box: 12);
                            h._FIELDSET();
                        }
                    });
                });
            }
            else
            {
                Node node = ac[this];
                ac.GiveFrame(200, false, 60 * 15, node.Label);
            }
        }

        [Ui("操作授权"), Style(ButtonOpen), Allow(OPRMGR)]
        public async Task grant(ActionContext ac, int cmd)
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
                        dc.Execute("UPDATE users SET oprat = NULL, opr = 0 WHERE tel = @1", p => p.Set(tel));
                    }
                }
                else if (cmd == 2) // add
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE tel = @3", p => p.Set(shopid).Set(opr).Set(tel));
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
                            m.RADIO(nameof(tel), tel, null, null, false, tel, name, Oprs[opr]);
                        }
                        m.T("</div>");
                        m.BUTTON(nameof(grant), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加人员");
                m.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                m.SELECT(nameof(opr), opr, Oprs, "权限");
                m.BUTTON(nameof(grant), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }
    }

    public class OprVarVarWork : Work
    {
        public OprVarVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("营业状态"), Style(ButtonShow, 1), Allow(OPRMEM)]
        public async Task status(ActionContext ac)
        {
            string shopid = ac[-1];
            short status;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        status = (short) dc.Scalar("SELECT status FROM shops WHERE id = @1", p => p.Set(shopid));
                        h.SELECT(nameof(status), status, Status);
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
            }
            ac.GivePane(200);
        }

        [Ui("客服上/下线"), Style(ButtonShow, 1)]
        public void seton(ActionContext ac)
        {
            string shopid = ac[-1];
            User prin = (User) ac.Principal;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        string hint = dc.Query1("SELECT 1 FROM shops WHERE id = @1 AND oprwx = @2", p => p.Set(shopid).Set(prin.wx)) ? "您当前已经是客服。是否下线，停止接收相关微信通知，并且我的电话不再显示为客服电话？" : "是否上线做客服，接收相关微信通知，并且我的电话显示为客服电话";
                        const bool yes = false;
                        h.CHECKBOX(nameof(yes), yes, hint, required: true);
                    }
                    h._FORM();
                });
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
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
        }

        [Ui("设置"), Style(ButtonShow)]
        public async Task sets(ActionContext ac)
        {
            string shopid = ac[-1];
            string schedule;
            string[] areas;
            decimal min, notch, off;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT schedule, areas, min, notch, off FROM shops WHERE id = @1", p => p.Set(shopid));
                    dc.Let(out schedule).Let(out areas).Let(out min).Let(out notch).Let(out off);
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.TEXT(nameof(schedule), schedule, "时间", box:12);
//                        h.SELECT(nameof(areas), areas,  "限送", box:12);
                        h.NUMBER(nameof(min), min, "起送", box:12);
                        h.NUMBER(nameof(notch), notch, "满额", box:12);
                        h.NUMBER(nameof(off), off, "扣减", box:12);
                        h._FORM();
                    });
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();

            schedule = f[nameof(schedule)];
            areas = f[nameof(areas)];
            min = f[nameof(min)];
            notch = f[nameof(notch)];
            off = f[nameof(off)];
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE shops SET schedule = @1, areas = @2, min = @3, notch = @4, off = @5 WHERE id = @6",
                    p => p.Set(schedule).Set(areas).Set(min).Set(notch).Set(off).Set(shopid));
            }
            ac.GivePane(200);
        }

        [Ui("功能照"), Style(ButtonCrop, Ordinals = 4)]
        public new async Task img(ActionContext ac, int ordinal)
        {
            string shopid = ac[-1];
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
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE shops SET img" + ordinal + " = @1 WHERE id = @2", p => p.Set(jpeg).Set(shopid));
            }
            ac.Give(200); // ok
        }
    }
}