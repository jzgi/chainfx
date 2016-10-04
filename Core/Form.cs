using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// An XML element that may contains child elements.
    /// </summary>
    public class Form : ISource
    {
        public bool Got(string name, out int v, int def = 0)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out decimal v, decimal def = 0)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out string v, string def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out DateTime v, DateTime def = default(DateTime))
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out Obj v, Obj def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out Arr v, Arr def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out byte[] v, byte[] def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out long v, long def = -1)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out short v, short def = 0)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out bool v, bool def = false)
        {
            throw new NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> v, List<T> def = null) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Got<T>(string name, out T v, T def = default(T)) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }
    }
}