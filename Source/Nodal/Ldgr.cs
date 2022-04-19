using System;
using System.Threading;

namespace Chainly.Nodal
{
    /// <summary>
    /// A ledger record in the distributed ledger system.
    /// </summary>
    public class Ldgr : IData, IKeyable<int>
    {
        public static readonly Ldgr Empty = new Ldgr();

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

        internal int seq;

        internal string acct; // account number

        internal string name; // account name

        internal DateTime created;
        internal string creator;

        internal decimal amt;

        internal decimal bal;

        // reference peer id
        internal short refpeerid;

        // reference sequence number
        internal int refseq;
        //
        // current block number

        internal volatile int blockid;

        internal Guid blockcs;
        internal Guid cs;
        internal DateTime stamp;


        public void Read(ISource s, short mask = 0xff)
        {
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);

            s.Get(nameof(cs), ref cs);
            s.Get(nameof(blockcs), ref blockcs);
            s.Get(nameof(stamp), ref stamp);
        }

        public void Write(ISink s, short mask = 0xff)
        {
            s.Put(nameof(seq), seq);
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
        }

        internal void IncrementBlockId()
        {
            Interlocked.Increment(ref blockid);
        }

        public int Key => seq;
    }
}