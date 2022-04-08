using System.Threading.Tasks;

namespace FabricQ.Nodal
{
    public interface INodeClient
    {
        Task<(int, NodeClientError)> InviteAsync(Peer peer);

        void ApproveAsk();

        void Quit();

        void ApproveQuit();


        void Discover();


        //
        // blockchain credit

        void Transfer();

        void Poll();
    }
}