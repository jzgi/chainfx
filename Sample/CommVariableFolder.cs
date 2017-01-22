using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// Each key is tne openid for a buyer or shop
    ///
    public class CommVariableFolder : WebFolder, IVariable
    {

        readonly ConcurrentDictionary<string, Inbox> inboxes;

        public CommVariableFolder(WebFolderContext fc) : base(fc)
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
            IToken tok = ac.Token;
            string userid = ac.Key;
            Inbox inbox = null;

            if (ac.GET)
            {
                if (!inboxes.TryGetValue(userid, out inbox)) // put in session
                {
                    // retrieve from database
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.QueryUn("SELECT * FROM inboxes WHERE userid = @1", p => p.Set(userid, false)))
                        {
                            inbox = dc.ToUn<Inbox>();
                        }
                        else
                        {
                            inbox = new Inbox();
                        }
                    }
                    inboxes.AddOrUpdate(userid, inbox, (k, v) => v);
                }
                // can wait (long polling)
                var messages = await inbox.GetAsync("wait".Equals(arg));
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

                if (inboxes.TryGetValue(userid, out inbox)) // if the user is active
                {
                    await inbox.Put(msg);
                }
                else // put in database
                {
                    using (var sc = Service.NewDbContext())
                    {
                        sc.Execute("INSERT INTO inboxes (from, to, ) VALUES () ON CONFLICT DO UPDATE", p => { });
                    }
                }
            }
        }


    }
}