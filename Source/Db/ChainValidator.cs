namespace SkyChain.Db
{
    public abstract class ChainValidator : IKeyable<(short, short)>
    {
        public (short, short) code { get; set; }

        protected abstract void Validate(Queuel[] tran);

        public ChainContext ChainCtx { get; set; }

        public (short, short) Key => code;
    }
}