namespace SkyChain.Chain
{
    public class Operation : Record
    {
        public new static readonly Operation Empty = new Operation();

        public const byte ID = 1, PRIVACY = 2;


        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {-1, "否决"},
            {0, null},
            {1, "进行"},
            {2, "通过"},
            {3, "存档"},
        };

        internal short value;
        internal string npeerid;
        internal string nan;
        internal short status;

        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);

            s.Get(nameof(value), ref value);
            s.Get(nameof(npeerid), ref npeerid);
            s.Get(nameof(nan), ref nan);
            s.Get(nameof(status), ref status);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);

            s.Put(nameof(value), value);
            s.Put(nameof(npeerid), npeerid);
            s.Put(nameof(nan), nan);
            s.Put(nameof(status), status);
        }
    }
}