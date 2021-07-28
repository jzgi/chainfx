using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainContext : DbContext
    {
        // whether a master context
        bool master;

        short remoteid;

        short id;

        short txtyp;

        // slave context id list
        ValueList<short> slaves;

        internal Peer local;

        internal Peer remote;

        public JObj In { get; set; }

        public JObj Out;

        internal ChainContext(DbSource src) : base(src)
        {
        }


        public async Task<bool> OperateAsync<D>(short peerid, string op, Action<IParameters> p = null, byte proj = 0x0f) where D : IData, new()
        {
            if (peerid == 0 || peerid == local.id) // call in- place
            {
                // local
                var lgc = ChainEnviron.GetLogic(txtyp);
                var o = lgc.GetOperation(op);

                // call
                if (o.IsAsync)
                {
                    await o.DoAsync(this);
                }
                else
                {
                    o.Do(this);
                }
            }
            else // call remote
            {
                var conn = ChainEnviron.GetClient(peerid);
                if (conn != null)
                {
                    // args
                    var cnt = new JsonContent(true, 1024);
                    cnt.Put(null, In);

                    // remote call
                    var (code, v) = await conn.RemoteCallAsync<D>(id, txtyp, op, cnt);
                }
                else
                {
                    throw new ChainException("");
                }
            }
            return false;
        }
    }
}