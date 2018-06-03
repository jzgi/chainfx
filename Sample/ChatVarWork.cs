using System.Threading.Tasks;
using Greatbone;
using static Samp.WeiXinUtility;

namespace Samp
{
    [User]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("发送"), Tool(Modal.Button)]
        public async Task reply(WebContext wc)
        {
            string orgid = wc[typeof(IOrgVar)];
            User prin = (User) wc.Principal;
            string wx = wc[this];

            string text = null;
            var f = await wc.ReadAsync<Form>();
            text = f[nameof(text)];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND wx = @2", p => p.Set(orgid).Set(wx)))
                {
                    dc.Let(out ChatMsg[] msgs);
                    msgs = msgs.AddOf(new ChatMsg {name = prin.name, text = text});
                    dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND wx = @3", p => p.Set(msgs).Set(orgid).Set(wx));
                }
            }
            await PostSendAsync(wx, "【商家消息】" + prin.name + "：" + text + "（http://144000.tv/org/" + orgid + "/）");
            wc.GiveRedirect("../");
        }
    }

    public class MyChatVarWork : ChatVarWork
    {
        public MyChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
        }
    }

    public class OprChatVarWork : ChatVarWork
    {
        public OprChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}