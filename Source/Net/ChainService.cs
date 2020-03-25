using CloudUn.Web;

namespace CloudUn.Net
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class ChainService : WebService
    {
        protected internal override void OnCreate()
        {
            CreateWork<AdmlyWork>("admly");
        }
        // inter-node


        // REST Data API
        public void token(WebContext wc)
        {
            // retrieve from idents
        }

        public void query(WebContext wc)
        {
        }

        public void querya(WebContext wc)
        {
        }

        public void put(WebContext wc)
        {
        }
    }
}