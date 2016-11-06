using System;

namespace Greatbone.Core
{

    /// <summary>
    /// To generate extended JSON document that may contain byte arrays.
    /// </summary>
    public class JxContent : JContent
    {
        const int Initialcapacity = 16 * 1024;

        public JxContent(int capacity = Initialcapacity) : base(true, capacity)
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