using System.Reflection;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public abstract class ChainLogic : IKeyable<short>
    {
        public short trantyp;

        // declared operations 
        readonly Map<string, ChainOperation> operations = new Map<string, ChainOperation>(32);

        protected ChainLogic(short trantyp)
        {
            this.trantyp = trantyp;

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
                ChainOperation op;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ChainContext))
                {
                    op = new ChainOperation(this, mi, async);
                }
                else
                {
                    continue;
                }

                operations.Add(op);
            }
        }

        public ChainOperation GetOperation(string name) => operations[name];

        public bool OnValidate(_Ety[] row)
        {
            return false;
        }


        public async Task<bool> check(ChainContext cc)
        {
            var args = cc.In;
            string acct = args[nameof(acct)];
            decimal amt = args[nameof(amt)];

            if (await cc.QueryTopAsync("SELECT amt FROM chain.archivals WHERE typ = @1 AND acct = @2 ORDER BY txn DESC LIMIT 1"))
            {
                cc.Let(out decimal lastamt);
                return lastamt > amt;
            }
            else
            {
                return false;
            }
        }

        public bool save(ChainContext cc)
        {
            return false;
        }

        public ChainLogic Row(string acct, string name, string remark, decimal amt)
        {
            var r = new _Ety()
            {
                typ = this.trantyp,
                acct = acct,
                name = name,
                remark = remark,
                amt = amt
            };

            return this;
        }

        public ChainLogic RemoteRow(string acct, string name, string remark, decimal amt)
        {
            var r = new _Ety()
            {
                typ = this.trantyp,
                acct = acct,
                name = name,
                remark = remark,
                amt = amt
            };

            return this;
        }

        public short Key => trantyp;
    }
}