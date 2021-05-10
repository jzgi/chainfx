namespace SkyChain.Db
{
    /// <summary>
    /// A workflow operational state or step.
    /// </summary>
    public class ChainOp : _State
    {
        public new static readonly ChainOp Empty = new ChainOp();

        public const byte ID = 1, PRIVACY = 2;

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
    }
}