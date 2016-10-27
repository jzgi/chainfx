using System;

namespace Greatbone.Core
{

    /// <summary>
    /// To generate extendd JSON document with byte array support.
    /// </summary>
    public class JxContent : JContent
    {

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