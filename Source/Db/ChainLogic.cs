namespace SkyChain.Db
{
    public class ChainLogic : IKeyable<short>
    {
        public short typ;

        long txn;


        private _Ety[] rows;

        public ChainLogic(short typ)
        {
            this.typ = typ;
        }


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

        public ChainLogic Txn(long txn)
        {
            this.txn = txn;
            return this;
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