using Greatbone;
using static Samp.User;

namespace Samp
{
    public abstract class ChatWork<V> : Work where V : ChatVarWork
    {
        protected ChatWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("联系客服")]
    [User]
    public class MyChatWork : ChatWork<OprChatVarWork>
    {
        public MyChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyChatVarWork, string>((obj) => ((Chat) obj).custwx);
        }

        public void @default(WebContext wc)
        {
            var orgs = Obtain<Map<string, Org>>();
            string orgid = wc.Query[nameof(orgid)];
            string wx = wc[-1];
            if (orgid == null)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.RADIOSET(nameof(orgid), orgid, orgs, legend: "选择护工站");
                    h._FORM();
                });
            }
            else
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Chat>("SELECT * FROM chats WHERE orgid = @1 AND wx = @2", p => p.Set(orgid).Set(wx));
                    if (o == null)
                    {
                        o = new Chat
                        {
                            orgid = orgid,
                            orgname = orgs[orgid].name,
                            custwx = wx,
                            custname = ((User) wc.Principal).name
                        };
                    }
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.T("<article>");
                        h.T(o.orgname).T("tel:#mp.weixin.qq.com");
                        for (int i = 0; i < o.msgs?.Length; i++)
                        {
                            var m = o.msgs[i];
                            h.P(m.text, m.name);
                        }
                        string text = null;
                        h.FORM_();
                        h.ROW_().TEXTAREA(nameof(text), text, tip: "输入文字", max: 100, required: true, w: 0x56).TOOL(nameof(OprChatVarWork.reply))._ROW();
                        h._FORM();
                        h.T("</article>");
                    });
                }
            }
        }
    }

    [Ui("客服")]
    [User(OPR)]
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
                        for (int i = 0; i < o.msgs?.Length; i++)
                        {
                            var m = o.msgs[i];
                            h.P(m.text);
                        }
                        string text = null;
                        h.FORM_();
                        h.ROW_().TEXTAREA(nameof(text), text, max: 100, tip: "输入文字", required: true).TOOL(nameof(OprChatVarWork.reply))._ROW();
                        h._FORM();
                    });
                });
            }
        }

        [Ui("删除"), Tool(Modal.ButtonConfirm)]
        public void del(WebContext wc)
        {
        }
    }
}