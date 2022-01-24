using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Store
{
    public class Peer : IData, IKeyable<short>
    {
        public static readonly Peer Empty = new Peer();

        public const short
            STA_DISABLED = 0,
            STA_SHOWED = 1,
            STA_ENABLED = 2,
            STA_PREFERED = 3;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STA_DISABLED, "禁用"},
            {STA_SHOWED, "展示"},
            {STA_ENABLED, "可用"},
            {STA_PREFERED, "优先"},
        };

        internal short id;

        // the specialized extensible discriminator
        internal short typ;

        // object status
        internal short status;

        // readable name
        internal string name;

        // desctiprive text
        internal string tip;

        internal DateTime created;

        // persona who created or lastly modified
        internal string creator;

        internal string domain; // remote uri

        internal bool secure; // remote uri

        //
        // current block number

        internal volatile int blockid;

        internal long blockcs;

        public Peer()
        {
        }

        public Peer(JObj s)
        {
            Read(s);
        }

        public void Read(ISource s, short proj = 0x0fff)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(status), ref status);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tip), ref tip);
            s.Get(nameof(created), ref created);
            s.Get(nameof(creator), ref creator);
            s.Get(nameof(domain), ref domain);
            s.Get(nameof(secure), ref secure);
        }

        public void Write(ISink s, short proj = 0x0fff)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(typ), typ);
            s.Put(nameof(status), status);
            s.Put(nameof(name), name);
            s.Put(nameof(tip), tip);
            s.Put(nameof(created), created);
            s.Put(nameof(creator), creator);
            s.Put(nameof(domain), domain);
            s.Put(nameof(secure), secure);
        }

        internal void IncrementBlockId()
        {
            Interlocked.Increment(ref blockid);
        }

        public short Key => id;

        public short Id => id;

        public string Name => name;

        public string Tip => tip;

        public short Status => status;

        public string Domain => domain;

        public DateTime Created => created;

        public bool IsRunning => status == STA_ENABLED;

        public int CurrentBlockId => blockid;

        internal async Task PeekLastBlockAsync(DbContext dc)
        {
            if (id > 0)
            {
                await dc.QueryTopAsync("SELECT seq, blockcs FROM chain.archive WHERE peerid = @1 ORDER BY seq DESC LIMIT 1", p => p.Set(Id));
                dc.Let(out long seq);
                dc.Let(out long bcs);
                if (seq > 0)
                {
                    var (bid, _) = FedUtility.ResolveSeq(seq);

                    blockid = bid;
                    blockcs = bcs;
                }
            }
        }
    }
}