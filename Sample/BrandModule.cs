using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandModule : WebModule, IAdmin
    {
        public BrandModule(IScope scope) : base(scope)
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