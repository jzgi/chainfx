using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// /123/Get
    /// /123/Put
    public class ChatVarHub : WebVarHub
    {
        private ConcurrentDictionary<string, List<Chat>> online;

        public ChatVarHub(WebBuild cfg) : base(cfg)
        {
        }

        ///<summary>Returns the administration UI.</summary>
        [IfAdmin]
        public override void @default(WebContext wc, string x)
        {
            base.@default(wc, x);
        }

        [IfSelf]
        public void get(WebContext wc, string userid)
        {
            List<Chat> chats;
            if (online.TryGetValue(userid, out chats)) // put in session
            {
                // return cached msgs
            }
            else
            {
                // database operation
                using (var dc = Service.NewDbContext())
                {
                    dc.QueryA("SELECT * FROM chats WHERE to=@to", p => p.Put("@to", userid));
                    // load into memory
                }
            }

            // wc.Response.SetContentAsJson(chats)
        }

        public void put(WebContext wc, string receiver)
        {
            IToken tok = wc.Token;
            string sender = tok.Key;
            string text = wc.ToString();

            List<Chat> chats;
            if (online.TryGetValue(receiver, out chats)) // put in session
            {
                Chat chat = chats.First(c => c.partner.Equals(sender));
                chats[0].Put(text);
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