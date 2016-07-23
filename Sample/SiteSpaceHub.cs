using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteSpaceHub : WebHub<Space>
    {
        public SiteSpaceHub(WebCreationContext wcc) : base(wcc)
        {
            AddSub<SiteSpaceMgtSub>("mgt", null);
        }


        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}