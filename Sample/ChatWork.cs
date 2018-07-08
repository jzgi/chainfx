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

    [UserAccess]
    [Ui("联系客服")]
    public class MyChatWork : ChatWork<OprChatVarWork>
    {
        public MyChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyChatVarWork, string>((obj) => ((Chat) obj).orgid);
        }

        public void @default(WebContext wc)
        {
            int myid = wc[-1];
            var prin = (User) wc.Principal;
            var orgs = Obtain<Map<string, Org>>();
            string orgid = wc.Query[nameof(orgid)];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Chat>("SELECT * FROM chats WHERE custid = @1 AND orgid = @2", p => p.Set(myid).Set(orgid));
                if (arr == null)
                {
                    arr = new[]
                    {
                        new Chat {orgid = orgid, custwx = prin.wx, custname = prin.name}
                    };
                }
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(title: Label);
                    h.BOARD(arr, o =>
                    {
                        h.T("<h4 class=\"uk-card-header\">").T(orgs[orgid].name).T("</h4>");
                        if (o.msgs != null)
                        {
                            h.T("<main class=\"uk-card-body\">");
                            for (int i = 0; i < o.msgs.Length; i++)
                            {
                                var m = o.msgs[i];
                                h.P(m.text, m.name);
                            }
                            h.T("</main>");
                        }
                        string text = null;
                        h.T("<footer class=\"uk-card-footer\">");
                        h.FORM_();
                        h.ROW_().TEXTAREA(nameof(text), text, tip: "输入文字", max: 100, required: true, w: 0x0f).TOOL(nameof(MyChatVarWork.say))._ROW();
                        h._FORM();
                        h.T("</footer>");
                    });
                });
            }
        }
    }

    [UserAccess(OPR)]
    [Ui("客服")]
    public class OprChatWork : ChatWork<OprChatVarWork>
    {
        public OprChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<OprChatVarWork, int>((obj) => ((Chat) obj).custid);
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
                        h.T("<header class=\"uk-accordion-title\">").T(o.custname).T("</header>");
                        if (o.msgs != null)
                        {
                            h.T("<main class=\"uk-accordion-content uk-grid\">");
                            for (int i = 0; i < o.msgs.Length; i++)
                            {
                                var m = o.msgs[i];
                                h.P(m.text, m.name);
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