using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainContext : DbContext
    {
        // whether a master context
        public bool Master { get; set; }

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


        public async Task<bool> CallAsync(short peerid, string op, Action<IParameters> p = null, byte proj = 0x0f)
        {
            if (peerid == 0 || peerid == local.id) // call in- place
            {
                // local
            }
            else // call remote
            {
                var conn = ChainEnv.GetClient(peerid);
                if (conn != null)
                {
                    // args
                    var cnt = new JsonContent(true, 1024);
                    cnt.Put(null, In);

                    // remote call
                    var (code, v) = await conn.CallAsync(id, txtyp, op, cnt);
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