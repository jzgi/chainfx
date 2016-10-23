using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandModule : WebModule, IAdmin
    {
        public BrandModule(WebArg arg) : base(arg)
        {
            SetMultiple<BrandMultiple>();
        }

        //
        // ADMIN
        //

        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void srch(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }
    }
}