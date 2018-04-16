using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class OrgVarWork : Work, IOrgVar
    {
        protected OrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class SampVarWork : OrgVarWork
    {
        public SampVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<SampItemVarWork, string>(obj => ((Item)obj).name);
        }

        public void icon(WebContext wc)
        {
            string orgid = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) wc.Give(204, @public: true, maxage: 3600); // no content 
                    else wc.Give(200, new StaticContent(byteas), @public: true, maxage: 3600);
                }
                else wc.Give(404, @public: true, maxage: 3600); // not found
            }
        }
    }

    public class AdmOrgVarWork : OrgVarWork
    {
        public AdmOrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext wc)
        {
            string orgid = wc[this];
            const byte proj = Org.ADM;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Org.Empty, proj).T(" FROM orgs WHERE id = @1");
                    var o = dc.Query1<Org>(p => p.Set(orgid), proj);
                    wc.GivePane(200, h =>
                    {
                        h.FORM_().FIELDSET_("填写网点信息");
                        h.STATIC(o.id, "编号");
                        h.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                        h.TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true);
                        h.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                        h.NUMBER(nameof(o.x), o.x, "经度").NUMBER(nameof(o.x), o.x, "纬度");
                        h._FIELDSET()._FORM();
                    });
                }
            }
            else // post
            {
                var o = await wc.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orgs")._SET_(Org.Empty, proj ^ Org.ID).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj ^ Org.ID);
                        p.Set(orgid);
                    });
                }
                wc.GivePane(200);
            }
        }

        [Ui("经理"), Tool(ButtonShow)]
        public async Task mgr(WebContext wc)
        {
            string orgid = wc[this];
            string wx_tel_name;
            if (wc.GET)
            {
                string forid = wc.Query[nameof(forid)];
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDSET_("查询帐号（手机号）");
                    m.SEARCH(nameof(forid), forid, pattern: "[0-9]+", max: 11, min: 11);
                    m._FIELDSET();
                    if (forid != null)
                    {
                        using (var dc = NewDbContext())
                        {
                            if (dc.Query1("SELECT concat(wx, ' ', tel, ' ', name) FROM users WHERE tel = @1", p => p.Set(forid)))
                            {
                                dc.Let(out wx_tel_name);
                                m.FIELDSET_("设置经理");
                                m.RADIO(nameof(wx_tel_name), wx_tel_name, wx_tel_name);
                                m._FIELDSET();
                            }
                        }
                    }
                    m._FORM();
                });
            }
            else // post
            {
                var f = await wc.ReadAsync<Form>();
                wx_tel_name = f[nameof(wx_tel_name)];
                (string wx, string tel, string name) = wx_tel_name.ToTriple();
                using (var dc = NewDbContext())
                {
                    dc.Execute(@"UPDATE orgs SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; 
                        UPDATE users SET opr = " + OPRMGR + ", oprat = @4 WHERE wx = @1;", p => p.Set(wx).Set(tel).Set(name).Set(orgid));
                }
                wc.GivePane(200);
            }
        }

        [Ui("照片"), Tool(ButtonCrop)]
        public async Task icon(WebContext wc)
        {
            string orgid = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) wc.Give(204); // no content 
                        else
                            wc.Give(200, new StaticContent(byteas));
                    }
                    else wc.Give(404); // not found           
                }
                return;
            }

            var f = await wc.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE orgs SET icon = @1 WHERE id = @2", p => p.Set(jpeg).Set(orgid));
            }
            wc.Give(200); // ok
        }
    }
}