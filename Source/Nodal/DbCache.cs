using System;

namespace ChainFx.Nodal
{
    internal abstract class DbCache
    {
        // the actual type cached 
        readonly Type typ;

        // bitwise match
        readonly short flag;

        // expiration period in seconds
        readonly int maxage;

        // either of the two forms
        protected readonly Delegate fetch;


        protected DbCache(Delegate fetch, Type typ, int maxage, short flag)
        {
            this.fetch = fetch;
            this.typ = typ;
            this.maxage = maxage;
            this.flag = flag;
        }


        public Delegate Fetch => fetch;

        public Type Typ => typ;

        public abstract bool IsAsync { get; }

        public int MaxAge => maxage;

        public short Flag => flag;
    }
}