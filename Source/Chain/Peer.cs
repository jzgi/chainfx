using System;
using System.Threading;

namespace SkyChain.Chain
{
    public class Peer : IData, IKeyable<short>
    {
        public static readonly Peer Empty = new Peer();

        public const short
            STATUS_VOID = 0,
            STATUS_RUNNING = 1;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STATUS_VOID, "停止"},
            {STATUS_RUNNING, "运行"}
        };

        internal short id;
        internal string name;
        internal string uri; // remote address
        internal DateTime created;
        internal bool native;
        internal short status;

        // current block number
        [NonSerialized] internal int blockid;

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
    }
}