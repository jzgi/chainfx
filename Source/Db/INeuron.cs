using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.FlowControl;

namespace SkyChain.Db
{
    public interface INeuron
    {
        void Input();

        void Output();
    }
}