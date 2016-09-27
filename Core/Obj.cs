using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Obj : IIn
    {
        readonly Roll<Element> pairs = new Roll<Element>(16);

        public bool Get(string name, ref int value)
        {
            Element pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref bool value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref T value) where T : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public Element this[string name]
        {
            get
            {
                return pairs[name];
            }
        }
    }
}