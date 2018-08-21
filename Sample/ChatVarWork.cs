using System;
using System.Linq;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    [UserAccess]
    public abstract class ChatVarWork : Work
    {
        public const short PicWidth = 360, PicHeight = 270;

        protected ChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class SampChatVarWork : ChatVarWork
    {
        public SampChatVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public async Task @default(WebContext wc)
        {
            int chatid = wc[this];
            string text = null;
            ArraySegment<byte> img;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Chat.Empty).T(" FROM chats WHERE id = @1");
                    var chat = dc.Query1<Chat>(p => p.Set(chatid));
                    wc.GivePage(200, h =>
                        {
                            h.BOARD(chat.posts, o =>
                            {
                                h.HEADER_("uk-card-header uk-flex-between uk-text-muted").SPAN_().T("<span uk-icon=\"user\"></span>&nbsp;").T(o.uname).SP().T(o.grpat)._SPAN().T(o.time)._HEADER();
                                h.MAIN_("uk-card-body");
                                h.H5(o.text);
                                if (o.img > 0)
                                {
                                    h.DIV_("uk-margin-auto").T("<img data-src=\"img-").T(o.img).T("\" width=\"").T(PicWidth).T("\" height=\"").T(PicHeight).T("\" uk-img>")._DIV();
                                }
                                h._MAIN();
                            }, css: "uk-card-primary");
                            h.FORM_(mp: true);
                            h.FIELDUL_();
                            h.LI_().TEXTAREA(null, nameof(text), text, tip: "填写跟帖内容", max: 400, required: true)._LI();
                            h._FIELDUL();
                            h.CROP(nameof(img), "图片（可选）", PicWidth, PicHeight);
                            h.BOTTOMBAR_().BUTTON(null, 0, "跟帖", css: "uk-button-primary")._BOTTOMBAR();
                            h._FORM();
                        }, true, 60
                    );
                }
            }
            else // POST
            {
                var now = DateTime.Now;
                var prin = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                text = f[nameof(text)];
                img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    dc.Query1("SELECT posts FROM chats WHERE id =@1", p => p.Set(chatid));
                    dc.Let(out Post[] posts);
                    var last = posts.Last(x => x.img > 0);
                    short ord = (short) (img.Count == 0 ? 0 : last.img + 1);
                    posts = posts.AddOf(new Post
                    {
                        uid = prin.id, uname = prin.name, grpat = prin.grpat, text = text, img = ord, time = now
                    });
                    if (img.Count > 0 && last.img < 10)
                    {
                        dc.Execute($"UPDATE chats SET posts = @1, posted = @2, fcount = @3, fname = @4, img{ord} = @5 WHERE id = @6", p => p.Set(posts).Set(now).Set(posts.Length - 1).Set(prin.name).Set(img).Set(chatid));
                    }
                    else
                    {
                        dc.Execute("UPDATE chats SET posts = @1, posted = @2, fcount = @3, fname = @4 WHERE id = @5", p => p.Set(posts).Set(now).Set(posts.Length - 1).Set(prin.name).Set(chatid));
                    }
                }
                wc.GiveRedirect();
            }
        }

        public void img(WebContext wc, int ordinal)
        {
            int chatid = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM chats WHERE id = @1", p => p.Set(chatid)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, 120);
                }
                else wc.Give(404, @public: true, maxage: 120); // not found
            }
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