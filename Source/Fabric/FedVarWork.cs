using ChainFx.Web;

namespace ChainFx.Fabric
{
    /// <summary>
    /// To delegate contents for peer nodes.
    /// </summary>
    /// <see cref="FedService"/>
    public class FedVarWork : WebWork
    {
        public void @default(WebContext wc)
        {
            short peerKey = wc[0];
            var p = Nodality.GetPeer(peerKey);
            
        }
    }
}