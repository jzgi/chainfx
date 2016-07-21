using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteSpaceMux : WebMux<Space>
    {

        protected internal override void Init()
        {

            AddSub<SiteSpaceMgtSub>("mgt", null);
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}