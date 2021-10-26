using System.Reflection;
using System.Threading.Tasks;
using SkyChain;

namespace SkyChain.Chain
{
    /// <summary>
    /// A contained set of actions runnable localy and remotely .
    /// </summary>
    public abstract class ChainBot
    {
        // declared operations 
        readonly Map<string, ChainOp> ops = new Map<string, ChainOp>(32);

        protected ChainBot()
        {
            // gather actions
            foreach (var mi in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // return task or void
                var ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task<bool>))
                {
                    async = true;
                }
                else if (ret == typeof(bool))
                {
                    async = false;
                }
                else
                {
                    continue;
                }

                // signature filtering
                var pis = mi.GetParameters();
                ChainOp op;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ChainContext))
                {
                    op = new ChainOp(this, mi, async);
                }
                else
                {
                    continue;
                }

                ops.Add(op);
            }
        }

        public ChainOp GetOp(string name) => ops[name];
    }
}