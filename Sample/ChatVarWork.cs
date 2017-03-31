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

        public ChatVarWork(WorkContext fc) : base(fc)
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
        [User]
        public async Task inbox(ActionContext ac, int arg)
        {
            User tok = (User)ac.Principal;
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
                            chat = dc.ToObject<Chat>();
                        }
                        else
                        {
                            chat = new Chat();
                        }
                    }
                    chats.AddOrUpdate(userid, chat, (k, v) => v);
                }
                // can wait (long polling)
                var messages = await chat.GetAsync("wait".Equals(arg));
                if (messages == null)
                {
                    ac.Give(204); // no content
                }
                else
                {
                    ac.Give(200, messages);
                }
            }
            else // post message(s) to inbox
            {
                var txt = await ac.ReadAsync<Str>();
                Message msg = new Message()
                {
                    fromid = tok.wx,
                    from = tok.name,
                    text = txt.ToString(),
                    time = DateTime.Now
                };

                if (chats.TryGetValue(userid, out chat)) // if the user is active
                {
                    await chat.Put(msg);
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