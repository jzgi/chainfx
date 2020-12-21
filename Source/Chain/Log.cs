namespace SkyChain.Chain
{
    public class Log : State
    {
        public new static readonly Log Empty = new Log();

        public const byte ID = 1, PRIVACY = 2;

        public const short
            CREATED = 0,
            FORTH = 1,
            BACK = 2,
            CANCEL = 3,
            DONE = 4;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CREATED, null},
            {FORTH, "推进"},
            {BACK, "退回"},
            {CANCEL, "撤销"},
            {DONE, "完成"},
        };


        internal string ppeer;
        internal string pacct;
        internal string pname;
        internal string npeer;
        internal string nacct;
        internal string nname;

        internal short status;


        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);

            s.Get(nameof(ppeer), ref ppeer);
            s.Get(nameof(pacct), ref pacct);
            s.Get(nameof(pname), ref pname);
            s.Get(nameof(npeer), ref npeer);
            s.Get(nameof(nacct), ref nacct);
            s.Get(nameof(nname), ref nname);
            s.Get(nameof(status), ref status);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);

            s.Put(nameof(ppeer), ppeer);
            s.Put(nameof(pacct), pacct);
            s.Put(nameof(pname), pname);
            s.Put(nameof(npeer), npeer);
            s.Put(nameof(nacct), nacct);
            s.Put(nameof(nname), nname);
            s.Put(nameof(status), status);
        }

        public bool IsLocal => ppeer == null;
    }
}