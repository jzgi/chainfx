namespace SkyChain.Nodal
{
    public interface INodeClient
    {
        void Ask();

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