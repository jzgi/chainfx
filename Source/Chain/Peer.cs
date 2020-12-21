using System;

namespace SkyChain.Chain
{
    public class Peer : IData, IKeyable<string>
    {
        public static readonly Peer Empty = new Peer();

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, null},
            {1, "暂停"},
            {2, "运行"}
        };

        internal string id;
        internal string name;
        internal string uri; // remote address
        internal DateTime created;
        internal bool local;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(uri), ref uri);
            s.Get(nameof(created), ref created);
            s.Get(nameof(local), ref local);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(uri), uri);
            s.Put(nameof(created), created);
            s.Put(nameof(local), local);
            s.Put(nameof(status), status);
        }

        public string Key => id;

        public string Name => name;

        public string Uri => uri;

        public bool IsLocal => local;
    }
}