namespace CloudUn.Net
{
    public class DataTyp : IData
    {
        public static readonly Map<short, string> Ops = new Map<short, string>()
        {
            {0, "QUERY"},
            {1, "STORE RESERVED"},
            {2, "STORE UNRESERVED"},
        };

        public static readonly Map<short, string> Statuses = new Map<short, string>()
        {
            {0, "Disabled"},
            {1, "Enabled"},
        };

        internal short id;

        internal string name;

        internal string conttyp;

        internal short op;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(conttyp), ref conttyp);
            s.Get(nameof(op), ref op);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(conttyp), conttyp);
            s.Put(nameof(op), op);
            s.Put(nameof(status), status);
        }
    }
}