using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// Each key is tne openid for a buyer or shop
    ///
    public class CommVarFolder : WebFolder, IVar
    {

        readonly ConcurrentDictionary<string, Chat> chats;

        public CommVarFolder(WebFolderContext fc) : base(fc)
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
        [Check]
        public async Task inbox(WebActionContext ac, string arg)
        {
            Token tok = (Token)ac.Token;
            string userid = ac.Var;
            Chat chat = null;

            if (ac.GET)
            {
                if (!chats.TryGetValue(userid, out chat)) // put in session
                {
                    // retrieve from database
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.QueryOne("SELECT * FROM inboxes WHERE userid = @1", p => p.Set(userid, false)))
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
                    ac.Reply(204); // no content
                }
                else
                {
                    ac.ReplyJson(200, messages);
                }
            }
            else // post message(s) to inbox
            {
                var txt = await ac.ReadAsync<Text>();
                Message msg = new Message()
                {
                    fromid = tok.Key,
                    from = tok.Name,
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