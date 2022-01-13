﻿using System;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    public class ChainContext : DbContext
    {
        readonly WebContext wc;

        readonly ChainClient client;

        internal Peer local;

        public JObj In { get; set; }

        public JObj Out;

        internal ChainContext(DbSource dbsource, WebContext wc, ChainClient client) : base(dbsource)
        {
            this.wc = wc;
            this.client = client;
        }

        public bool IsRemote => client != null;

        public async Task SetState(string table, long id, short state, bool seal = false)
        {
            var org = wc.Party;
            var user = wc.Principal.ToString();

            Sql("UPDATE ").T(table).T(" SET state = @1 WHERE id = @2");
            await QueryAsync(p => p.Set(state).Set(id));

            if (IsRemote)
            {
                // remote call to update corresponding record
            }
        }

        public async Task UnsetState(long id, short state)
        {
        }

        public async Task<bool> CallAsync(short peerid, string op, Action<IParameters> p = null, short proj = 0x0fff)
        {
            if (peerid == 0 || peerid == local.id) // call in- place
            {
                // local
            }
            else // call remote
            {
                var conn = Chain.GetClient(peerid);
                if (conn != null)
                {
                    // args
                    var cnt = new JsonContent(true, 1024);
                    cnt.Put(null, In);

                    // remote call
                    var (code, v) = await conn.CallAsync(0, 0, op, cnt);
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