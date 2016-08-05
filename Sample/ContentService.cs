using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The content management service.
	///
    public class ContentService : WebService
    {
        //
        // INIT
        //

        public ContentService(WebServiceContext wsc) : base(wsc)
        {
        }

        //
        // REQUEST HANDLING
        //

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }

        //
        // EVENT HANDLING
        //

        public void OnEnroll()
        {
        }
    }
}