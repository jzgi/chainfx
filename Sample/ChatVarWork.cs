using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [User]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprChatVarWork : ChatVarWork
    {
        public OprChatVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("回复", Mode = UiMode.ButtonShow)]
        public async Task reply(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            User prin = (User)ac.Principal;
            string wx = ac[this];

            string text = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXTAREA(nameof(text), text, "发送信息", max: 30, required: true);
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                text = f[nameof(text)];
                ChatMsg[] msgs;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT msgs FROM chats WHERE shopid = @1 AND wx = @2", p => p.Set(shopid).Set(wx)))
                    {
                        dc.Let(out msgs);
                        msgs = msgs.AddOf(new ChatMsg() { name = prin.nickname, text = text });
                        dc.Execute("UPDATE chats SET msgs = @1 WHERE shopid = @2 AND wx = @3", p => p.Set(msgs).Set(shopid).Set(wx));
                    }
                }
                await WeiXinUtility.PostSendAsync(wx, "[商家]" + prin.nickname + "：" + text);
                ac.GivePane(200);
            }
        }

        static readonly Func<IData, bool> ALL = obj => ((Chat)obj).msgs?.Length > Chat.NUM;

        [Ui("显示更多", Mode = UiMode.AnchorOpen)]
        public void all(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string wx = ac[this];
            ac.GivePane(200, m =>
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT msgs FROM chats WHERE shopid = @1 AND wx = @2", p => p.Set(shopid).Set(wx)))
                    {
                        ChatMsg[] msgs;
                        dc.Let(out msgs);
                        m.CARD_();
                        for (int i = 0; i < msgs.Length; i++)
                        {
                            ChatMsg msg = msgs[i];
                            m.CARDITEM(msg.name, msg.text);
                        }
                        m._CARD();
                    }
                }
            });
        }
    }
}