namespace SkyChain.Chain
{
    /// <summary>
    /// A workflow operational state or step.
    /// </summary>
    public class FlowOp : FlowState, IDualKeyable<long, short>
    {
        public new static readonly FlowOp Empty = new FlowOp();

        public const byte ID = 1, PRIVACY = 2;

        public const short
            STATUS_STARTED = 0b000,
            STATUS_ABORTED = 0b001,
            STATUS_FORTHIN = 0b010,
            STATUS_FORTHOUT = 0b011,
            STATUS_BACKIN = 0b100,
            STATUS_BACKOUT = 0b101,
            STATUS_ENDED = 0b111;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STATUS_STARTED, "开始"},
            {STATUS_ABORTED, "撤销"},
            {STATUS_FORTHIN, "可批"},
            {STATUS_FORTHOUT, "待批"},
            {STATUS_BACKIN, "退回"},
            {STATUS_BACKOUT, "回退"},
            {STATUS_ENDED, "已批"},
        };

        internal short status;

        public override void Read(ISource s, byte proj = 15)
        {
            base.Read(s, proj);
            s.Get(nameof(status), ref status);
        }

        public override void Write(ISink s, byte proj = 15)
        {
            base.Write(s, proj);
            s.Put(nameof(status), status);
        }

        public short Status => status;

        public bool IsPresent => (status & 0b001) == 0;

        public long Key => job;

        public (long, short) CompositeKey => (job, step);
    }
}