using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
     public class Inbox : TaskCompletionSource<WebActionContext>, IData
    {
        internal string userid;

        internal int status;

        internal Message[] msgs;

        internal WebActionContext context;

        internal Message[] Get(WebActionContext ac)
        {
            context = ac;

            return null;
        }

        internal void Put(string msg)
        {
            // msgs.Add(new Message());
        }

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(userid), ref userid);
            src.Get(nameof(status), ref status);
            src.Get(nameof(msgs), ref msgs);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(userid), userid);
            snk.Put(nameof(status), status);
            snk.Put(nameof(msgs), msgs);
        }
    }

}