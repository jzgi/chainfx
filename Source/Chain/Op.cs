namespace SkyChain.Chain
{
    public class Op : BlockOp, IDualKeyable<long, short>
    {
        public new static readonly Op Empty = new Op();

        public const byte ID = 1, PRIVACY = 2;

        public const short
            STARTED = 0b000,
            ABORTED = 0b001,
            FORWARD_IN = 0b010,
            FORWARD_OUT = 0b011,
            BACKWARD_IN = 0b100,
            BACKWARD_OUT = 0b101,
            ENDED = 0b111;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STARTED, "开始"},
            {ABORTED, "撤销"},
            {FORWARD_IN, "送入"},
            {FORWARD_OUT, "送出"},
            {BACKWARD_IN, "退来"},
            {BACKWARD_OUT, "退走"},
            {ENDED, "结束"},
        };


        internal short ppeerid;
        internal string pacct;
        internal string pname;
        internal short npeerid;
        internal string nacct;
        internal string nname;

        internal short status;


        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);

            s.Get(nameof(ppeerid), ref ppeerid);
            s.Get(nameof(pacct), ref pacct);
            s.Get(nameof(pname), ref pname);
            s.Get(nameof(npeerid), ref npeerid);
            s.Get(nameof(nacct), ref nacct);
            s.Get(nameof(nname), ref nname);
            s.Get(nameof(status), ref status);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);

            s.Put(nameof(ppeerid), ppeerid);
            s.Put(nameof(pacct), pacct);
            s.Put(nameof(pname), pname);
            s.Put(nameof(npeerid), npeerid);
            s.Put(nameof(nacct), nacct);
            s.Put(nameof(nname), nname);
            s.Put(nameof(status), status);
        }

        public bool IsLocal => ppeerid == 0;

        public short PPeerId => ppeerid;

        public string PAcct => pacct;

        public string PName => pname;

        public short NPeerId => npeerid;

        public string NAcct => nacct;

        public string NName => nname;

        public short Status => status;

        public bool IsPresent => (status & 0b001) == 0;

        public (long, short) CompositeKey => (job, step);
    }
}