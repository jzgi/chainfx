using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    [UserAccess]
    public abstract class ChatVarWork : Work
    {
        protected ChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class SampChatVarWork : ChatVarWork
    {
        public SampChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            int chatid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Chat.Empty).T(" FROM chats WHERE id = @1");
                var chat = dc.Query1<Chat>(p => p.Set(chatid));
                wc.GivePage(200, h =>
                    {
                        h.LIST(chat.posts, o =>
                        {
                            h.DIV_("uk-col uk-link-heading uk-padding-small");
                            h.H4_().T(o.text)._H4();
                            h.FI(null, o.posted);
                            h._DIV();
                        });
                    }, true, 60
                );
            }
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
                var msg = new Post {uname = prin.name, text = text};
                if (dc.Query1("SELECT msgs FROM chats WHERE orgid = @1 AND custid = @2", p => p.Set(orgid).Set(prin.id)))
                {
                    dc.Let(out Post[] msgs);
                    msgs = msgs.AddOf(msg, limit: 10);
                    dc.Execute("UPDATE chats SET msgs = @1, quested = localtimestamp WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(prin.id));
                }
                else
                {
                    var o = new Chat()
                    {
                        uid = prin.id,
                        uname = prin.name,
                        posts = new[] {msg},
                        posted = DateTime.Now
                    };
                    dc.Sql("INSERT INTO chats")._(Chat.Empty)._VALUES_(Chat.Empty);
                    dc.Execute(p => o.Write(p));
                }
            }
            wc.GiveRedirect("../?orgid=" + orgid);
        }
    }

    public class CtrChatVarWork : ChatVarWork
    {
        public CtrChatVarWork(WorkConfig cfg) : base(cfg)
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
                wc.GivePane(200, h => { h.FORM_().FIELDUL_().TEXTAREA(null, nameof(text), text, max: 100, min: 1)._FIELDUL()._FORM(); });
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
                        dc.Let(out custwx).Let(out Post[] msgs);
                        msgs = msgs.AddOf(new Post {uname = prin.name, text = text}, limit: 10);
                        dc.Execute("UPDATE chats SET msgs = @1 WHERE orgid = @2 AND custid = @3", p => p.Set(msgs).Set(orgid).Set(custid));
                    }
                }
                await ((SampService) Service).WeiXin.PostSendAsync(custwx, "【" + orgs[orgid].name + "】" + text + "<a href=\"" + SampUtility.NETADDR + "/my//chat/?orgid=" + orgid + "\">（去回复）</a>");
                wc.GivePane(200);
            }
        }
    }
}