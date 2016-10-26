using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A form object model parsed from www-form-urlencoded.
    /// </summary>
    public class Form : ISource
    {
        const int InitialCapacity = 16;

        readonly Roll<Pair> pairs;

        public Form(int capacity = InitialCapacity)
        {
            pairs = new Roll<Pair>(16);
        }

        internal void Add(string name, string v)
        {
            Pair e = new Pair()
            {
            };
            pairs.Add(e);
        }

        public int Count => pairs.Count;

        public Pair this[int index] => pairs[index];

        public Pair this[string name] => pairs[name];


        //
        // SOURCE
        //

        public bool Got(string name, ref bool v)
        {
            Pair p;
            if (pairs.TryGet(name, out p))
            {
                v = false;
                return false;
            }
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


        public bool Got<P>(string name, ref P[] v, uint x = 0) where P : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Got<P>(string name, ref P v, uint x = 0) where P : IPersist, new()
        {
            throw new NotImplementedException();
        }
    }
    
}