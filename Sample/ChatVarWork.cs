using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.WeiXinUtility;

namespace Samp
{
    [UserAccess]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyChatVarWork : ChatVarWork
    {
        public MyChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("发送"), Tool(Button)]
        public async Task say(WebContext wc)
        {
            string orgid = wc[this];
            var orgs = Obtain<Map<string, Org>>();
            User prin = (User) wc.Principal;
            string text = null;
            var f = await wc.ReadAsync<Form>();
            text = f[nameof(text)];
            using (var dc = NewDbContext())
            {
                var msg = new Msg {name = prin.name, text = text};
                if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND custid = @2", p => p.Set(orgid).Set(prin.id)))
                {
                    dc.Let(out Msg[] msgs);
                    msgs = msgs.AddOf(msg);
                    dc.Execute("UPDATE chats SET msgs = @1, quested = localtimestamp WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(prin.id));
                }
                else
                {
                    var o = new Chat()
                    {
                        orgid = orgid,
                        custid = prin.id,
                        custname = prin.name,
                        custwx = prin.wx,
                        msgs = new[] {msg},
                        quested = DateTime.Now
                    };
                    dc.Sql("INSERT INTO chats")._(Chat.Empty)._VALUES_(Chat.Empty);
                    dc.Execute(p => o.Write(p));
                }
            }
            wc.GiveRedirect("../?orgid=" + orgid);
        }
    }

    public class OprChatVarWork : ChatVarWork
    {
        public OprChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("发送"), Tool(Button)]
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
                    dc.Let(out Msg[] msgs);
                    msgs = msgs.AddOf(new Msg {name = prin.name, text = text});
                    dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND wx = @3", p => p.Set(msgs).Set(orgid).Set(wx));
                }
            }
            await PostSendAsync(wx, "【商家消息】" + prin.name + "：" + text + "（http://144000.tv/org/" + orgid + "/）");
            wc.GiveRedirect("../");
        }
    }
}