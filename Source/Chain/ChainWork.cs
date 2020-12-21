using System;
using SkyChain.Web;

namespace SkyChain.Chain
{
    public abstract class ChainWork : WebWork
    {
        // ordinal of the step
        internal short step;

        // last validation check
        internal DateTime last;

        public short Step => step;

        public string Name { get; set; }

        public int Cycle { get; set; } = 30;

        /// <summary>
        /// only called from the validator thread 
        /// </summary>
        internal bool IsNewCycle(DateTime now)
        {
            var elapsed = (now - last).Seconds;
            if (elapsed > Cycle)
            {
                last = now;
                return true;
            }
            return false;
        }

        void CreateRemoteOrDb(string an, string ln, string descr, decimal amt, decimal bal, JObj doc = null)
        {
            var op = new Log
            {
                acct = an,
                ldgr = ln,
                descr = descr,
                amt = amt,
                bal = bal,
                doc = doc,
            };
            if (op.IsLocal)
            {
            }
        }

        //
        //


        /// <summary>
        /// Called in the validator thread of current chain node.
        /// </summary>
        /// <returns>A value indicating the progress, with a full of 100, a denial of 0</returns>
        public virtual short OnValidate(Log op)
        {
            return 100;
        }
    }
}