using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ChatWork<V> : Work where V : ChatVarWork
    {
        protected ChatWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class SampChatWork : ChatWork<SampChatVarWork>
    {
        public SampChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<SampChatVarWork, string>((obj) => ((Chat) obj).ctrid);
        }

        public void @default(WebContext wc, int page)
        {
            string ctrid = wc[-1];
            var org = Obtain<Map<string, Org>>()[ctrid];
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ Chat.DETAIL;
                dc.Sql("SELECT ").collst(Chat.Empty, proj).T(" FROM chats WHERE ctrid = @1");
                var arr = dc.Query<Chat>(p => p.Set(ctrid), proj);
                wc.GivePage(200, h =>
                    {
                        h.TOPBAR(false);

                        h.LIST(arr, o =>
                        {
                            h.COL_(0x23, css: "uk-padding-small");
                            h.T("<h3>").T(o.uname).T("</h3>");
                            h.P(o.posted);
                            h.ROW_();
                            h.FORM_(css: "uk-width-auto");
                            h.HIDDEN(nameof(ctrid), ctrid);
                            h._FORM();
                            h._ROW();
                            h._COL();
                        });

                        h.BOTTOMBAR_().TOOL(nameof(@new))._BOTTOMBAR();
                    }, true, 60
                );
            }
        }

        [Ui("发新帖"), Tool(ButtonShow, size: 2)]
        public async Task @new(WebContext wc)
        {
            string ctrid = wc[-1];
            string subject = null;
            string text = null;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_(mp: true);
                    h.TEXT(nameof(subject), text, max: 20, tip: "输入文字");
                    h.TEXTAREA(nameof(text), text, max: 500, tip: "输入文字");
                    h._FORM();
                });
            }
            else
            {
                var prin = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                text = f[nameof(text)];
                subject = f[nameof(subject)];

                var chat = new Chat
                {
                    ctrid = ctrid,
                    subject = subject,
                    uid = prin.id,
                    uname = prin.name,
                    msgs = new[]
                    {
                        new Msg
                        {
                            uid = prin.id,
                            uname = prin.name,
                            text = text
                        }
                    },
                    posted = DateTime.Now
                };
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ Chat.ID;
                    dc.Sql("INSERT INTO chats")._(chat, proj)._VALUES_(chat, proj);
                    dc.Execute(p => chat.Write(p));
                }
                wc.GivePane(200);
            }
        }
    }

    [UserAccess(OPR)]
    [Ui("客服")]
    public class CtrChatWork : ChatWork<CtrChatVarWork>
    {
        public CtrChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<CtrChatVarWork, int>((obj) => ((Chat) obj).uid);
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Chat>("SELECT * FROM chats WHERE orgid = @1", p => p.Set(orgid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.ACCORDION(arr, o =>
                    {
                        h.T("<header class=\"uk-accordion-title\">").T(o.uname).T("</header>");
                        if (o.msgs != null)
                        {
                            h.T("<main class=\"uk-accordion-content uk-grid\">");
                            for (int i = 0; i < o.msgs.Length; i++)
                            {
                                var m = o.msgs[i];
                                h.P(m.text, m.uname);
                            }
                            h.VARTOOLS();
                            h.T("</main>");
                        }
                    });
                });
            }
        }

        [Ui("删除"), Tool(ButtonConfirm)]
        public void del(WebContext wc)
        {
        }
    }
}