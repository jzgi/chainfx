namespace SkyChain.Db
{
    public abstract class ChainValidator : IKeyable<short>
    {
        public short typ { get; set; }

        protected abstract void Validate(_Row[] tran);

        public ChainContext ChainCtx { get; set; }

        public short Key => typ;
    }
}