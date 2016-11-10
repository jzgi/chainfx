using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandDir : WebDir
    {
        public BrandDir(WebDirContext wnc) : base(wnc)
        {
            SetVariable<BrandVariableDir>();
        }

        public void @default(WebContext wc, string subscpt)
        {
        }

        //
        // ADMIN
        //

        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void mgmt(WebContext wc, string subscpt)
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