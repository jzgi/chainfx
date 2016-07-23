using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BusinessService : WebService
    {
        //
        // INIT
        //

        public BusinessService(WebCreationContext wcc) : base(wcc)
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