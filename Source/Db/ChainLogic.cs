using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainLogic : IKeyable<short>
    {
        // declared ops 
        readonly Map<string, ChainOp> ops = new Map<string, ChainOp>(32);

        public short typ;

        public ChainLogic(short typ)
        {
            this.typ = typ;
        }

        public ChainOp GetOp(string name) => ops[name];

        public bool OnValidate(_Ety[] row)
        {
            return false;
        }


        public bool OnStart(_Ety row)
        {
            return false;
        }

        public bool OnEnd(_Ety row)
        {
            return false;
        }


        public ChainLogic Typ(short typ)
        {
            this.typ = typ;
            return this;
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

        public void save(ChainContext cc)
        {
        }

        public ChainLogic Row(string acct, string name, string remark, decimal amt)
        {
            var r = new _Ety()
            {
                typ = this.typ,
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
                typ = this.typ,
                acct = acct,
                name = name,
                remark = remark,
                amt = amt
            };

            return this;
        }

        public short Key => typ;
    }
}