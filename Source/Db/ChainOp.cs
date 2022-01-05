using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainOp : IKeyable<string>
    {
        // the declaring logic
        readonly ChainDuty bean;

        // coding name
        readonly string name;

        // description
        readonly string tip;

        readonly bool async;

        // forms of the operation method
        //
        readonly Func<ChainContext, bool> @do;
        readonly Func<ChainContext, Task<bool>> doAsync;

        internal ChainOp(ChainDuty drive, MethodInfo mi, bool async)
        {
            this.bean = drive;
            this.name = mi.Name;
            this.tip = name == string.Empty ? "./" : name;
            this.async = async;


            // create a doer delegate
            if (async)
            {
                doAsync = (Func<ChainContext, Task<bool>>) mi.CreateDelegate(typeof(Func<ChainContext, Task<bool>>), drive);
            }
            else
            {
                @do = (Func<ChainContext, bool>) mi.CreateDelegate(typeof(Func<ChainContext, bool>), drive);
            }
        }

        public ChainDuty Drive => bean;

        public Func<ChainContext, bool> Do => @do;

        public Func<ChainContext, Task<bool>> DoAsync => doAsync;

        public string Name => name;

        public string Key => name;

        public string Tip => tip;

        public bool IsAsync => async;
    }
}