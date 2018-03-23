using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Core.User;

namespace Core
{
    public abstract class OrgVarWork : Work, IOrgVar
    {
        protected OrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class CoreVarWork : OrgVarWork
    {
        public CoreVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<CoreItemVarWork, string>(obj => ((Item) obj).name);
        }

        public void icon(WebContext ac)
        {
            string orgid = ac[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204, @public: true, maxage: 3600); // no content 
                    else ac.Give(200, new StaticContent(byteas), @public: true, maxage: 3600);
                }
                else ac.Give(404, @public: true, maxage: 3600); // not found
            }
        }
    }

    public class AdmOrgVarWork : OrgVarWork
    {
        public AdmOrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext ac)
        {
            string orgid = ac[this];
            const byte proj = Org.ADM;
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Org.Empty, proj).T(" FROM orgs WHERE id = @1");
                    var o = dc.Query1<Org>(p => p.Set(orgid), proj);
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.FIELD(o.id, "编号");
                        m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                        m.TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true);
                        m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                        m.NUMBER(nameof(o.x), o.x, "经度", width: 6).NUMBER(nameof(o.x), o.x, "纬度", width: 6);
                        m._FORM();
                    });
                }
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orgs")._SET_(Org.Empty, proj ^ Org.ID).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj ^ Org.ID);
                        p.Set(orgid);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("经理"), Tool(ButtonShow)]
        public async Task mgr(WebContext ac)
        {
            string orgid = ac[this];
            string wx_tel_name;
            if (ac.GET)
            {
                string forid = ac.Query[nameof(forid)];
                ac.GivePane(200, m =>
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
                var f = await ac.ReadAsync<Form>();
                wx_tel_name = f[nameof(wx_tel_name)];
                (string wx, string tel, string name) = wx_tel_name.ToTriple();
                using (var dc = NewDbContext())
                {
                    dc.Execute(@"UPDATE orgs SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; 
                        UPDATE users SET opr = " + OPRMGR + ", oprat = @4 WHERE wx = @1;", p => p.Set(wx).Set(tel).Set(name).Set(orgid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("照片"), Tool(ButtonCrop)]
        public new async Task icon(WebContext ac)
        {
            string orgid = ac[this];
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                            ac.Give(200, new StaticContent(byteas));
                    }
                    else ac.Give(404); // not found           
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE orgs SET icon = @1 WHERE id = @2", p => p.Set(jpeg).Set(orgid));
            }
            ac.Give(200); // ok
        }
    }
}