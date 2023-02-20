using System;
using System.Threading.Tasks;

namespace ChainFx.Nodal
{
    /// <summary>
    /// An encapsulation of relevant resources for domestic or inter-node ledger transaction.
    /// </summary>
    public class LdgrContext : DbContext
    {
        // connector to remote peer, can be null when domestic
        readonly NodeClient connector;

        internal LdgrContext(NodeClient connector)
        {
            this.connector = connector;
        }

        public bool IsDemostic => connector == null;

        public async Task SetState(string table, long id, short state, bool seal = false)
        {
            Sql("UPDATE ").T(table).T(" SET state = @1 WHERE id = @2");
            await QueryAsync(p => p.Set(state).Set(id));

            if (IsDemostic)
            {
                // remote call to update corresponding record
            }
        }

        public async Task UnsetState(long id, short state)
        {
        }

        public async Task<bool> TieAsync()
        {
            return false;
        }


        public async Task<bool> CallAsync(short peerid, string op, Action<IParameters> p = null, short proj = 0xff)
        {
            var self = Nodality.Self;
            if (peerid == 0 || peerid == self.id) // call in- place
            {
                // local
            }
            else // call remote
            {
                var conn = Nodality.GetConnector(peerid);
                if (conn != null)
                {
                    // args
                    var cnt = new JsonBuilder(true, 1024);

                    // remote call
                    // var (code, v) = await conn.CallAsync(0, 0, op, cnt);
                }
                else
                {
                    throw new LdgrException("");
                }
            }
            return false;
        }

        public async void AcceptAsync(Node node)
        {
        }

        public async Task TransferAsync(string srcAcct, int v, string targAcct)
        {
            // deduct source account (always local)
            Sql("INSERT ledgrs_ (seq, acct, name, v, bal, rpeerid, racct) VALUES () ");
            await ExecuteAsync();

            // target account
            if (IsDemostic)
            {
                Sql("INSERT ledgrs_ (seq, acct, name, v, bal, rpeerid, racct) VALUES () ");
            }
            else // remote
            {
                await connector.TransferAsync(targAcct, 0, 0);
            }
        }
    }
}