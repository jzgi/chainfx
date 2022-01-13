using System;

namespace SkyChain.Chain
{
    public struct ChainStep
    {
        string org { get; set; }

        string role { get; set; }

        string op { get; set; }

        string user { get; set; }

        private DateTime stamp { get; set; }
    }
}