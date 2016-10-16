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
            SetVarHub<BrandVarHub>(false);
        }

        //
        // ADMIN
        //

        public void del(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void search(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc)
        {
            throw new NotImplementedException();
        }
    }
}