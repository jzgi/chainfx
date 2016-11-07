using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandModuleDo : AbstModuleDo, IMgmt
    {
        public BrandModuleDo(WebArg arg) : base(arg)
        {
            SetMux<BrandVarDo>();
        }

        public override void @default(WebContext wc, string subscpt)
        {
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