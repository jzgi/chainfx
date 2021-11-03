using System.Reflection;
using System.Threading.Tasks;
using SkyChain;

namespace SkyChain.Chain
{
    /// <summary>
    /// A contained set of actions runnable localy and remotely .
    /// </summary>
    public abstract class ChainDrive
    {
        // declared operations 
        readonly Map<string, ChainAction> ops = new Map<string, ChainAction>(32);

        protected ChainDrive()
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
                ChainAction action;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ChainContext))
                {
                    action = new ChainAction(this, mi, async);
                }
                else
                {
                    continue;
                }

                ops.Add(action);
            }
        }

        public ChainAction GetOp(string name) => ops[name];
    }
}