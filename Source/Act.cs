using System;

namespace SkyChain
{
    public struct Act
    {
        string user { get; set; }

        string role { get; set; }

        string party { get; set; }

        string op { get; set; }

        private DateTime stamp { get; set; }
    }
}