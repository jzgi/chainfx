using System;
using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class MsgXHub : WebXHub
	{
		private ConcurrentDictionary<string, Chat> hanging;

		public MsgXHub(WebServiceContext wsc) : base(wsc)
		{
		}

		///<summary>Returns the administration UI.</summary>
		[Admin]
		public override void Default(WebContext wc, string x)
		{
			base.Default(wc, x);
		}

		[Self]
		public void Get(WebContext wc, string x)
		{
		}

		public void Put(WebContext wc, string receiver)
		{
			IToken tok = wc.Token;
			string sender = tok.Login;
			string text = wc.ToString();

			Chat chat;
			if (hanging.TryGetValue(receiver, out chat)) // put in session
			{
				chat.Put(text);
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