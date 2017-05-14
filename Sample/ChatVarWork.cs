using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// Each key is tne openid for a buyer or shop
    ///
    public class ChatVarWork : Work
    {
        readonly ConcurrentDictionary<string, Chat> chats;

        public ChatVarWork(WorkContext wc) : base(wc)
        {
        }

        ///
        /// Retrieve or put message(s).
        ///
        /// <code>
        /// GET /-userid-/inbox[-wait]
        /// </code>
        /// <code>
        /// POST /-userid-/inbox
        /// </code>
        ///
        public async Task inbox(ActionContext ac, int arg)
        {
            User tok = (User) ac.Principal;
            string userid = ac[0];
            Chat chat = null;

            if (ac.GET)
            {
                if (!chats.TryGetValue(userid, out chat)) // put in session
                {
                    // retrieve from database
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM inboxes WHERE userid = @1", p => p.Set(userid)))
                        {
                            chat = dc.ToData<Chat>();
                        }
                        else
                        {
                            chat = new Chat();
                        }
                    }
                    chats.AddOrUpdate(userid, chat, (k, v) => v);
                }
                // can wait (long polling)
            }
            else // post message(s) to inbox
            {
                var txt = await ac.ReadAsync<Str>();
                ChatMsg msg = new ChatMsg()
                {
                    fromid = tok.wx,
                    from = tok.nickname,
                    text = txt.ToString(),
                    time = DateTime.Now
                };

                if (chats.TryGetValue(userid, out chat)) // if the user is active
                {
//                    await chat.Put(msg);
                }
                else // put in database
                {
                    using (var sc = Service.NewDbContext())
                    {
                        sc.Execute("INSERT INTO chats (from, to, ) VALUES () ON CONFLICT DO UPDATE", p => { });
                    }
                }
            }
        }
    }
}