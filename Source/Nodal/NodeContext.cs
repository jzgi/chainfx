using System;
using System.Threading.Tasks;

namespace SkyChain.Nodal
{
    public class NodeContext : DbContext
    {
        internal Peer self;

        readonly NodeClient connector;

        public JObj In { get; set; }

        public JObj Out;

        internal NodeContext(DbSource dbSource, NodeClient connector) : base(dbSource)
        {
            this.connector = connector;
        }

        public bool IsRemote => connector != null;

        public async Task SetState(string table, long id, short state, bool seal = false)
        {
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

        public async Task<bool> TieAsync()
        {
            return false;
        }


        public async Task<bool> CallAsync(short peerid, string op, Action<IParameters> p = null, short proj = 0xff)
        {
            if (peerid == 0 || peerid == self.id) // call in- place
            {
                // local
            }
            else // call remote
            {
                var conn = Home.GetConnector(peerid);
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
                    throw new NodeException("");
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
            peer.id = Home.Self.id;
            var (code, err) = await connector.InviteAsync(peer);
        }

        public async void AcceptAsync(Peer peer)
        {
        }

        public async void TransferAsync(Peer peer)
        {
        }
    }
}