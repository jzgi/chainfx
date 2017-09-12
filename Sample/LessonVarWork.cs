using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [User]
    public abstract class LessonVarWork : Work
    {
        protected LessonVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class AdmChatVarWork : ChatVarWork
    {
        public AdmChatVarWork(WorkContext wc) : base(wc)
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
                    m.TEXT(nameof(text), text, "发送信息", pattern: "[\\S]*", max: 30, required: true);
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
                        msgs = msgs.AddOf(new ChatMsg { name = prin.name, text = text });
                        dc.Execute("UPDATE chats SET msgs = @1 WHERE shopid = @2 AND wx = @3", p => p.Set(msgs).Set(shopid).Set(wx));
                    }
                }
                await WeiXinUtility.PostSendAsync(wx, "【商家消息】" + prin.name + "：" + text + "（http://shop.144000.tv/pub/" + shopid + "/）");
                ac.GivePane(200);
            }
        }
    }
}