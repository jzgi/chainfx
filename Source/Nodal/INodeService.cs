using SkyChain.Web;

namespace SkyChain.Nodal
{
    public interface INodeService
    {
        void onask(WebContext wc);

        void onaccept(WebContext wc);

        void onquit(WebContext wc);

        void onapprovequit(WebContext wc);


        /// <summary>
        /// Return list of nodes in the federal network
        /// </summary>
        /// <param name="wc"></param>
        void discover(WebContext wc);
        
        
        
        void ontransfer(WebContext wc);
        void onpoll(WebContext wc);
    }
}