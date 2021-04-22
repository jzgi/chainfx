using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainPeer : IData, IKeyable<short>
    {
        public static readonly ChainPeer Empty = new ChainPeer();

        public const short
            STATUS_STOPPED = 0,
            STATUS_RUNNING = 1;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STATUS_STOPPED, "已停止"},
            {STATUS_RUNNING, "运行中"}
        };

        internal short id;
        internal string name;
        internal string uri; // remote address
        internal DateTime created;
        internal bool native;
        internal short status;

        // current block number
        internal volatile int blockid;
        internal long blockcs;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(uri), ref uri);
            s.Get(nameof(created), ref created);
            s.Get(nameof(native), ref native);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(uri), uri);
            s.Put(nameof(created), created);
            s.Put(nameof(native), native);
            s.Put(nameof(status), status);
        }

        internal void IncrementBlockId()
        {
            Interlocked.Increment(ref blockid);
        }

        public short Key => id;

        public short Id => id;

        public string Name => name;

        public string Uri => uri;

        public DateTime Created => created;

        public bool Native => native;

        public bool IsRunning => status == STATUS_RUNNING;

        public int CurrentBlockId => blockid;

        internal async Task PeekLastBlockAsync(DbContext dc)
        {
            if (id > 0)
            {
                await dc.QueryTopAsync("SELECT seq, blockcs FROM chain.blocks WHERE pid = @1 ORDER BY seq DESC LIMIT 1", p => p.Set(Id));
                dc.Let(out long seq);
                dc.Let(out long bcs);
                if (seq > 0)
                {
                    var (bid, _) = ChainUtility.ResolveSeq(seq);

                    blockid = bid;
                    blockcs = bcs;
                }
            }
        }
    }
}