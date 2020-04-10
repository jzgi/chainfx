namespace SkyCloud.Chain
{
    public class Typ : IData, IKeyable<short>
    {
        public static readonly Map<short, string> Ops = new Map<short, string>()
        {
            {0, "GET"},
            {1, "PUT PRIVATE"},
            {2, "PUT PROTECTED"},
            {3, "PUT PUBLIC"},
        };

        public static readonly Map<short, string> Statuses = new Map<short, string>()
        {
            {0, "Disabled"},
            {1, "Enabled"},
        };

        internal short id;

        internal string name;

        internal string contentyp;

        internal short op;

        internal string contract;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(contentyp), ref contentyp);
            s.Get(nameof(op), ref op);
            s.Get(nameof(contract), ref contract);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(contentyp), contentyp);
            s.Put(nameof(op), op);
            s.Put(nameof(contract), contract);
            s.Put(nameof(status), status);
        }

        public short Key => id;
    }
}