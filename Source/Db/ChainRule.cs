using System;

namespace SkyChain.Db
{
    public abstract class ChainRule : IKeyable<(short, short)>
    {
        (short, short) code;

        protected abstract void Process(ChainContext ctx);

        public (short, short) Key => code;
    }
}