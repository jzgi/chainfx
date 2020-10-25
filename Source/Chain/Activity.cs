using SkyChain.Db;

namespace SkyChain.Chain
{
    public abstract class Activity
    {
        // the parent transaction descriptor
        internal TransactDefinition parent;

        // ordinal of the step
        internal short step;

        public TransactDefinition Parent => parent;

        public short Step => step;

        public string Name { get; set; }

        public int Period { get; set; } = 30;

        public bool IsStart => step == 1;

        public bool IsEnd => step == parent.Size;

        //
        //

        public virtual void OnDefine()
        {
        }

        /// <summary>
        /// Called in an inputter thread.
        /// </summary>
        /// <param name="op">The operation record passed in</param>
        /// <returns>true indicates succeeded.</returns>
        public virtual bool OnSubmit(Operation op, DbContext dc, bool remote)
        {
            return true;
        }

        /// <summary>
        /// Called in the validator thread of current chain node.
        /// </summary>
        /// <param name="op">The operation record passed in</param>
        /// <returns>A value indicating the progress, with a full of 100, a denial of 0</returns>
        public virtual short OnValidate(Operation op)
        {
            return 100;
        }
    }
}