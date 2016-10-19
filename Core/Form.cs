using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A form object model parsed from www-form-urlencoded.
    /// </summary>
    public class Form : ISource
    {
        public bool Got(string name, ref bool v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref short v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref int v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref long v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref decimal v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref Number v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref string v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }


        public bool Got<T>(string name, ref T[] v, uint x = 0) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Got<T>(string name, ref T v, uint x = 0) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }
    }
}