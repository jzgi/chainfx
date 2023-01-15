using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static ChainFx.Fabric.NodeUtility;
using WebClient = ChainFx.Web.WebClient;

namespace ChainFx.Fabric
{
    /// <summary>
    /// A client connector to the specific federated peer.
    /// </summary>
    public class NodeClient : WebClient, IKeyable<short>
    {
        const int REQUEST_TIMEOUT = 3;

        const short
            NORMAL = 1,
            NETWORK_ERROR = 2,
            DATA_ERROR = 3,
            PEER_ERROR = 4,
            INTERNAL_ERROR = 5;

        public static readonly Map<short, string> States = new Map<short, string>
        {
            {0, null},
            {NORMAL, "normal"},
            {NETWORK_ERROR, "network error"},
            {DATA_ERROR, "data error"},
            {PEER_ERROR, "peer error"},
            {INTERNAL_ERROR, "internal error"},
        };

        // the remote peer
        readonly Peer peer;

        // acceptable remote addresses
        readonly IPAddress[] addrs;

        // the state of connectivity & operation
        short state;

        string err;

        public Ldgr[] arr;

        internal volatile int blockid;

        internal long blockcs;

        internal async Task PeekLastBlockAsync(DbContext dc)
        {
            await dc.QueryTopAsync("SELECT seq, blockcs FROM ledgrs_ WHERE peerid = @1 ORDER BY seq DESC LIMIT 1", p => p.Set(peer.Id));
            dc.Let(out long seq);
            dc.Let(out long bcs);
            if (seq > 0)
            {
                var (bid, _) = ResolveSeq(seq);

                blockid = bid;
                blockcs = bcs;
            }
        }

        internal void IncrementBlockId()
        {
            Interlocked.Increment(ref blockid);
        }


        /// <summary>
        /// To construct a node client. 
        /// </summary>
        internal NodeClient(Peer peer, NodeClientHandler handler = null) : base(peer.weburl, handler ?? new NodeClientHandler())
        {
            this.peer = peer;
            try
            {
                addrs = Dns.GetHostAddresses(BaseAddress.Host);
            }
            catch (Exception e)
            {
                // ServerEnviron.WAR(e.Message);
            }
            Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT);
        }


        public short Key => peer.id;

        public Peer Peer => peer;

        public string Err => err;

        const string
            CONTENT_TYPE = "Content-Type",
            CONTENT_LENGTH = "Content-Length",
            AUTHORIZATION = "Authorization";


        #region TIE-MGT

        public async Task<(int, NodeClientError)> AskAsync(Peer peer)
        {
            try
            {
                // request
                var req = new HttpRequestMessage(HttpMethod.Get, "/onask");

                req.Headers.TryAddWithoutValidation(X_FROM, Nodality.Self.id.ToString());
                req.Headers.TryAddWithoutValidation(X_CRYPTO, peer.id.ToString());

                var jc = new JsonBuilder(true, 1024);
                peer.Write(jc);

                req.Content = null;
                req.Headers.TryAddWithoutValidation(CONTENT_TYPE, jc.CType);
                req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, jc.Count.ToString());

                // response
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return ((int) rsp.StatusCode, 0);
                }
                var hdrs = rsp.Content.Headers;

                return (200, 0);
            }
            catch
            {
                state = NETWORK_ERROR;
                return (0, 0);
            }
        }

        public async Task<(int, NodeClientError)> AmswerAsync(short peerid, bool yes)
        {
            using var dc = Nodality.NewDbContext();

            // load the peer record
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM _peers_ WHERE id = @1");
            var obj = await dc.QueryAsync(p => p.Set(peerid));

            // check status

            // change states
            dc.Sql("UPDATE _peers_ SET tie = @1 WHERE id = @2");
            await dc.ExecuteAsync(p => p.Set(0));

            // notify the asker


            return (0, 0);
        }

        #endregion

        #region LDGR-OP

        internal async Task TransferAsync(string acct, short typ, int v)
        {
        }

        #endregion

        #region LDGR-REP

        public Task<int> ReplicateAsync()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region DIR

        public void DiscoverAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}