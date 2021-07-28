using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainOperation : IKeyable<string>
    {
        // the governing logic
        readonly ChainLogic logic;

        readonly string name;

        // relative path
        readonly string relative;

        readonly bool async;

        readonly VerAttribute ver;

        // 4 possible forms of the action method
        //
        readonly Func<ChainContext, bool> @do;
        readonly Func<ChainContext, Task<bool>> doAsync;

        internal ChainOperation(ChainLogic logic, MethodInfo mi, bool async)
        {
            this.logic = logic;
            this.name = mi.Name == "default" ? string.Empty : mi.Name;
            this.relative = name == string.Empty ? "./" : name;
            this.async = async;

            ver = (VerAttribute) mi.GetCustomAttribute(typeof(VerAttribute), true);

            // create a doer delegate
            if (async)
            {
                doAsync = (Func<ChainContext, Task<bool>>) mi.CreateDelegate(typeof(Func<ChainContext, Task<bool>>), logic);
            }
            else
            {
                @do = (Func<ChainContext, bool>) mi.CreateDelegate(typeof(Func<ChainContext, bool>), logic);
            }
        }

        public ChainLogic Logic => logic;

        public Func<ChainContext, bool> Do => @do;

        public Func<ChainContext, Task<bool>> DoAsync => doAsync;

        public string Name => name;

        public string Key => name;

        public string Relative => relative;

        public bool IsAsync => async;

        public VerAttribute Ver => ver;
    }
}