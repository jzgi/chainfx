using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandService : WebService
    {
        //
        // INIT
        //

        public BrandService(WebServiceContext wsc) : base(wsc)
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