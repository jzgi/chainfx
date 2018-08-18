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
            CreateVar<SampChatVarWork, int>((obj) => ((Chat) obj).id);
        }

        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ Chat.DETAIL;
                dc.Sql("SELECT ").collst(Chat.Empty, proj).T(" FROM chats ORDER BY top, posted DESC OFFSET @1 LIMIT 20");
                var arr = dc.Query<Chat>(p => p.Set(page * 20), proj);
                wc.GivePage(200, h =>
                    {
                        h.TOPBAR(false);
                        h.LIST(arr, o =>
                        {
                            h.T("<a class=\"uk-col uk-link-heading uk-padding-small\" href=\"").T(o.id).T("/\" onclick=\"return dialog(this, 8, false, 2, '").T(o.subject).T("');\">");
                            h.H4(o.subject);
                            h.FI(null, o.posted);
                            h.T("</a>");
                        });
                        h.BOTTOMBAR_().TOOL(nameof(@new))._BOTTOMBAR();
                    }, true, 60
                );
            }
        }

        [Ui("发新帖"), Tool(ButtonShow, css: "uk-button-primary", size: 2)]
        public async Task @new(WebContext wc)
        {
            string subject = null;
            string text = null;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_(mp: true);
                    h.FIELDUL_();
                    h.LI_().TEXT(null, nameof(subject), text, tip: "填写标题", max: 20, required: true)._LI();
                    h.LI_().TEXTAREA(null, nameof(text), text, tip: "填写文字内容", max: 500, required: true)._LI();
                    h._FIELDUL();
                    h.CROP("img", "附图片", 360, 270);
                    h._FORM();
                });
            }
            else // POST
            {
                var prin = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                text = f[nameof(text)];
                subject = f[nameof(subject)];

                var chat = new Chat
                {
                    subject = subject,
                    uid = prin.id,
                    uname = prin.name,
                    posts = new[]
                    {
                        new Post
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

    [UserAccess(CTR)]
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
                        if (o.posts != null)
                        {
                            h.T("<main class=\"uk-accordion-content uk-grid\">");
                            for (int i = 0; i < o.posts.Length; i++)
                            {
                                var m = o.posts[i];
                                h.FI(m.uname, m.text);
                            }
                            h.VARTOOLPAD();
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