using System;

namespace Greatbone.Core
{

    /// <summary>
    /// To generate extended JSON document that may contain byte arrays.
    /// </summary>
    public class JxContent : JContent
    {
        const int Initialcapacity = 16 * 1024;

        public JxContent(bool pooled, int capacity = Initialcapacity) : base(true, pooled, capacity)
        {
        }

        public override JContent Put(string name, byte[] v)
        {
            return this;
        }

        public override JContent Put(string name, ArraySegment<byte> v)
        {
            return this;
        }

    }

}