using System;
using System.Threading.Tasks;

namespace Chainly.Nodal
{
    /// <summary>
    /// An encapsulation of relevant resources for domestic or inter-node transaction flows.
    /// </summary>
    public class FlowContext : DbContext
    {
        // connector to remote peer, can be null when domestic
        readonly NodalClient connector;

        internal FlowContext(NodalClient connector)
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
                    var cnt = new JsonContent(true, 1024);

                    // remote call
                    var (code, v) = await conn.CallAsync(0, 0, op, cnt);
                }
                else
                {
                    throw new NodalException("");
                }
            }
            return false;
        }

        public async void InviteAsync(Peer peer)
        {
            // insert local record
            Sql("INSERT INTO peers_").colset(Peer.Empty)._VALUES_(Peer.Empty);
            await ExecuteAsync(p => peer.Write(p));

            // remote req
            peer.id = Nodality.Self.id;
            var (code, err) = await connector.InviteAsync(peer);
        }

        public async void AcceptAsync(Peer peer)
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
                await connector.AddTargetAccountAsync(targAcct, 0, 0);
            }
        }
    }
}