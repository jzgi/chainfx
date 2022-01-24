using System;

namespace SkyChain.Store
{
    public struct Act : IData
    {
        string user { get; set; }

        string role { get; set; }

        string party { get; set; }

        string op { get; set; }

        private DateTime stamp { get; set; }

        public void Read(ISource s, short proj = 4095)
        {
        }

        public void Write(ISink s, short proj = 4095)
        {
        }
    }
}