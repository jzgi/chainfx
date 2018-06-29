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
        public async Task say(WebContext wc)
        {
            string orgid = wc[-1];
            var orgs = Obtain<Map<string, Org>>();
            int custid = wc[this];
            User prin = (User) wc.Principal;
            string text = null;
            var f = await wc.ReadAsync<Form>();
            text = f[nameof(text)];
            string custwc = null;
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT custwc, msgs FROM chats WHERE orgid = @1 AND custid = @2", p => p.Set(orgid).Set(custid)))
                {
                    dc.Let(out custwc).Let(out Msg[] msgs);
                    msgs = msgs.AddOf(new Msg {name = prin.name, text = text});
                    dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(custid));
                }
            }
            await PostSendAsync(custwc, orgs[orgid].name, text, SampUtility.NETADDR + "/my//chat/?orgid=" + orgid);
            wc.GiveRedirect("../");
        }
    }
}