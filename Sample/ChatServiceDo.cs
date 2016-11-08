using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>The chat servoce.</summary>
    ///
    public class ChatServiceDo : AbstServiceDo
    {
        // the ongoing chat sessions, keyed by receiver's ID
        private ConcurrentDictionary<string, Wrap> chats = new ConcurrentDictionary<string, Wrap>();

        public ChatServiceDo(WebConfig cfg) : base(cfg)
        {
            SetMux<ChatMuxDo>();
        }

        public void Get(WebContext wc)
        {
            Wrap w = new Wrap()
            {
                key = "123",
                tcs = new TaskCompletionSource<int>()
            };
            chats.TryAdd("123", w);
        }

        public void Bar(WebContext wc)
        {


        }

        internal struct Wrap
        {
            internal string key;

            internal TaskCompletionSource<int> tcs;

        }

        struct Session
        {
            internal TaskCompletionSource<string> com;
        }

    }

}