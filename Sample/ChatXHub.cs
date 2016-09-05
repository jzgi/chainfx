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

        public ChatXHub(WebServiceContext wsc) : base(wsc)
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
                    sc.DoQuery("SELECT * FROM chats WHERE to=@to", p => { p.Add("@to", rcv); });
                    // load into memory
                }
            }

            // return
            JsonContent c = new JsonContent(null);
            c.Write(chats);

            wc.Response.Content = c;
        }

        public void Put(WebContext wc, string receiver)
        {
            IToken tok = wc.Token;
            string sender = tok.Login;
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
                    sc.DoNonQuery("INSERT INTO chats (from, to, ) VALUES () ON CONFLICT DO UPDATE", p => { });
                }
            }
        }
    }
}