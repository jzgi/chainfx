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
            CreateVar<MyChatVarWork, string>((obj) => ((Chat) obj).custwx);
        }

        public void @default(WebContext wc)
        {
            int myid = wc[-1];
            var prin = (User) wc.Principal;
            var orgs = Obtain<Map<string, Org>>();
            string orgid = wc.Query[nameof(orgid)];
            using (var dc = NewDbContext())
            {
                Chat[] arr;
                if (orgid == null)
                {
                    arr = dc.Query<Chat>("SELECT * FROM chats WHERE custid = @1", p => p.Set(myid));
                }
                else
                {
                    arr = dc.Query<Chat>("SELECT * FROM chats WHERE custid = @1 AND orgid = @2", p => p.Set(myid).Set(orgid));
                    if (arr == null)
                    {
                        arr = new[]
                        {
                            new Chat
                            {
                                orgid = orgid,
                                custwx = prin.wx,
                                custname = prin.name
                            }
                        };
                    }
                }
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                    {
                        h.T("<article>");
                        h.T("<header class=\"uk-card-header\">").T(orgs[orgid].name).T("</header>");
                        h.T("<main class=\"uk-card-body\">");
                        for (int i = 0; i < o.msgs?.Length; i++)
                        {
                            var m = o.msgs[i];
                            h.P(m.text, m.name);
                        }
                        string text = null;
                        h.FORM_();
                        h.ROW_().TEXTAREA(nameof(text), text, tip: "输入文字", max: 100, required: true, w: 0x56).TOOL(nameof(OprChatVarWork.reply))._ROW();
                        h._FORM();
                        h.T("</main>");
                        h.T("</article>");
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
            CreateVar<OprChatVarWork, string>((obj) => ((Chat) obj).custwx);
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[typeof(IOrgVar)];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Chat>("SELECT * FROM chats WHERE orgid = @1", p => p.Set(orgid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                    {
                        h.T("<article>");
                        h.T("<header class=\"uk-card-header\">").T(o.custname).T("</header>");
                        h.T("<main class=\"uk-card-body\">");
                        for (int i = 0; i < o.msgs?.Length; i++)
                        {
                            var m = o.msgs[i];
                            h.P(m.text, m.name);
                        }
                        string text = null;
                        h.FORM_();
                        h.ROW_().TEXTAREA(nameof(text), text, tip: "输入文字", max: 100, required: true, w: 0x56).TOOL(nameof(OprChatVarWork.reply))._ROW();
                        h._FORM();
                        h.T("</main>");
                        h.T("</article>");
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