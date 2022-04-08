using System;
using System.Threading;
using System.Threading.Tasks;

namespace FabricQ.Nodal
{
    public class Peer : Info, IKeyable<short>
    {
        public static readonly Peer Empty = new Peer();

        // 
        public const short
            TYP_NORM = 1,
            TYP_REPLICA = 2;

        public const short
            FED_SENT = 0,
            FED_RECEIVED = 1,
            FED_DENIED = 2,
            FED_ACCEPTED = 3;

        public static readonly Map<short, string> Feds = new Map<short, string>
        {
            {FED_SENT, "已请求"},
            {FED_RECEIVED, "接收"},
            {FED_DENIED, "已拒绝"},
            {FED_ACCEPTED, "已接受"},
        };

        internal short id;

        internal string domain; // remote uri

        internal bool secure; // remote uri

        internal short fed;

        internal string secret;

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

        public sealed override void Read(ISource s, short mask = 0xff)
        {
            base.Read(s, mask);

            s.Get(nameof(id), ref id);
            s.Get(nameof(domain), ref domain);
            s.Get(nameof(secure), ref secure);
            s.Get(nameof(fed), ref fed);
            s.Get(nameof(secret), ref secret);
        }

        public override void Write(ISink s, short mask = 0xff)
        {
            base.Write(s, mask);


            s.Put(nameof(id), id);
            s.Put(nameof(domain), domain);
            s.Put(nameof(secure), secure);
            s.Put(nameof(fed), fed);
            s.Put(nameof(secret), secret);
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
                    var (bid, _) = NodeUtility.ResolveSeq(seq);

                    blockid = bid;
                    blockcs = bcs;
                }
            }
        }
    }
}