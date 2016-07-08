using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpSpaceMux : WebMux<Space>
    {
        public OpSpaceMux(WebHub parent) : base(parent)
        {
            AddSub<OpSpaceMgtSub>("mgt", null);
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}