using System;

namespace Skyiah.Chain
{
    public class Record : IData
    {
        public static readonly Record Empty = new Record();

        public const byte ID = 1, PRIVACY = 2;

        // types
        public static readonly Map<short, string> Ops = new Map<short, string>
        {
            {0, "APP"},
            {1, "admin"},
        };

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "Disabled"},
            {1, "Enabled"},
        };

        internal string txpeerid;

        internal int txid;

        internal short op;

        internal DateTime stamp;

        internal short typ;

        internal string descr;

        internal string key;

        internal decimal amt;

        internal decimal balance;

        internal IData doc;

        internal string rtpeerid; // related peer id

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(txpeerid), ref txpeerid);
            s.Get(nameof(txid), ref txid);
            s.Get(nameof(op), ref op);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(key), ref key);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(balance), ref balance);
            s.Get(nameof(rtpeerid), ref rtpeerid);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(txpeerid), txpeerid);
            s.Put(nameof(txid), txid);
            s.Put(nameof(op), op);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(typ), typ);
            s.Put(nameof(key), key);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(balance), balance);
            s.Put(nameof(rtpeerid), rtpeerid);
            s.Put(nameof(status), status);
        }
    }
}