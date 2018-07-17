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
                var msg = new Msg {uname = prin.name, text = text};
                if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND custid = @2", p => p.Set(orgid).Set(prin.id)))
                {
                    dc.Let(out Msg[] msgs);
                    msgs = msgs.AddOf(msg, limit: 10);
                    dc.Execute("UPDATE chats SET msgs = @1, quested = localtimestamp WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(prin.id));
                }
                else
                {
                    var o = new Chat()
                    {
                        ctrid = orgid,
                        uid = prin.id,
                        uname = prin.name,
                        uwx = prin.wx,
                        msgs = new[] {msg},
                        posted = DateTime.Now
                    };
                    dc.Sql("INSERT INTO chats")._(Chat.Empty)._VALUES_(Chat.Empty);
                    dc.Execute(p => o.Write(p));
                }
            }
            wc.GiveRedirect("../?orgid=" + orgid);
        }
    }

    public class CtrlyChatVarWork : ChatVarWork
    {
        public CtrlyChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("回复"), Tool(ButtonShow)]
        public async Task say(WebContext wc)
        {
            string orgid = wc[-2];
            var orgs = Obtain<Map<string, Org>>();
            int custid = wc[this];
            User prin = (User) wc.Principal;
            string text = null;
            if (wc.GET)
            {
                wc.GivePane(200, h => { h.FORM_().FIELDSET_().TEXTAREA(nameof(text), text, max: 100, min: 1)._FIELDSET()._FORM(); });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                text = f[nameof(text)];
                string custwx = null;
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT custwx, msgs FROM chats WHERE orgid = @1 AND custid = @2", p => p.Set(orgid).Set(custid)))
                    {
                        dc.Let(out custwx).Let(out Msg[] msgs);
                        msgs = msgs.AddOf(new Msg {uname = prin.name, text = text}, limit: 10);
                        dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(custid));
                    }
                }
                await PostSendAsync(custwx, "【" + orgs[orgid].name + "】" + text + "<a href=\"" + SampUtility.NETADDR + "/my//chat/?orgid=" + orgid + "\">（去回复）</a>");
                wc.GivePane(200);
            }
        }
    }
}