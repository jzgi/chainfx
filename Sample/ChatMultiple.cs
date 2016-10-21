using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// /123/Get
    /// /123/Put
    public class ChatMultiple : WebMultiple
    {
        private ConcurrentDictionary<string, List<Chat>> online;

        public ChatMultiple(WebArg arg) : base(arg)
        {
        }

        ///<summary>Returns the administration UI.</summary>
        [IfAdmin]
        public override void @default(WebContext wc, string subscript)
        {
            base.@default(wc, subscript);
        }

        [IfSelf]
        public void get(WebContext wc, string subscpt)
        {
            List<Chat> chats;
            if (online.TryGetValue(subscpt, out chats)) // put in session
            {
                // return cached msgs
            }
            else
            {
                // database operation
                using (var dc = Service.NewDbContext())
                {
                    dc.QueryA("SELECT * FROM chats WHERE to=@to", p => p.Put("@to", subscpt));
                    // load into memory
                }
            }

            // wc.Response.SetContentAsJson(chats)
        }

        public void put(WebContext wc, string subscpt)
        {
            IToken tok = wc.Token;
            string sender = tok.Key;
            string text = wc.ToString();

            List<Chat> chats;
            if (online.TryGetValue(subscpt, out chats)) // put in session
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