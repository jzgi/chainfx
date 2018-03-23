using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Core
{
    [User]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class OprChatVarWork : ChatVarWork
    {
        public OprChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("回复"), Tool(ButtonShow)]
        public async Task reply(WebContext wc)
        {
            string orgid = wc[typeof(OrgVarWork)];
            User prin = (User) wc.Principal;
            string wx = wc[this];

            string text = null;
            if (wc.GET)
            {
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(text), text, "发送信息", pattern: "[\\S]*", max: 30, required: true);
                    m._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                text = f[nameof(text)];
                ChatMsg[] msgs;
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND wx = @2", p => p.Set(orgid).Set(wx)))
                    {
                        dc.Let(out msgs);
                        msgs = msgs.AddOf(new ChatMsg {name = prin.name, text = text});
                        dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND wx = @3", p => p.Set(msgs).Set(orgid).Set(wx));
                    }
                }
                await WeiXinUtility.PostSendAsync(wx, "【商家消息】" + prin.name + "：" + text + "（http://144000.tv/org/" + orgid + "/）");
                wc.GivePane(200);
            }
        }

        static readonly Func<IData, bool> ALL = obj => ((Chat) obj).msgs?.Length > Chat.NUM;

        [Ui("显示更多"), Tool(AnchorOpen)]
        public void all(WebContext wc)
        {
            string orgid = wc[typeof(OrgVarWork)];
            string wx = wc[this];
            wc.GivePane(200, m =>
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND wx = @2", p => p.Set(orgid).Set(wx)))
                    {
                        ChatMsg[] msgs;
                        dc.Let(out msgs);
                        for (int i = 0; i < msgs.Length; i++)
                        {
                            ChatMsg msg = msgs[i];
//                            m.CARDITEM(msg.name, msg.text);
                        }
                    }
                }
            });
        }
    }
}