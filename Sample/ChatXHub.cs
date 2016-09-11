using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// /123/Get
    /// /123/Put
    public class ChatXHub : WebXHub
    {
        private ConcurrentDictionary<string, List<Chat>> online;

        public ChatXHub(WebSubConf wsi) : base(wsi)
        {
        }

        ///<summary>Returns the administration UI.</summary>
        [Admin]
        public override void Default(WebContext wc, string x)
        {
            base.Default(wc, x);
        }

        [Self]
        public void Get(WebContext wc, string rcv)
        {
            List<Chat> chats;
            if (online.TryGetValue(rcv, out chats)) // put in session
            {
                // return cached msgs
            }
            else
            {
                // database operation
                using (var sc = Service.NewSqlContext())
                {
                    sc.QueryA("SELECT * FROM chats WHERE to=@to", p => p.Set("@to", rcv));
                    // load into memory
                }
            }

            // wc.Response.SetContentAsJson(chats)
        }

        public void Put(WebContext wc, string receiver)
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
                using (var sc = Service.NewSqlContext())
                {
                    sc.Execute("INSERT INTO chats (from, to, ) VALUES () ON CONFLICT DO UPDATE", p => { });
                }
            }
        }
    }
}