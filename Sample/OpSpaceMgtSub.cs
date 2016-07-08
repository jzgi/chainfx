using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpSpaceMgtSub : WebSub<Space>
    {
        public OpSpaceMgtSub(WebMux<Space> mux) : base(mux)
        {
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}