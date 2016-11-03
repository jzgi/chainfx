using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A form object model parsed from x-www-form-urlencoded.
    /// </summary>
    ///
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
            Pair pair = new Pair(name, v);
            pairs.Add(pair);
        }

        public int Count => pairs.Count;

        public Pair this[int index] => pairs[index];

        public Pair this[string name] => pairs[name];


        //
        // SOURCE
        //

        public bool Get(string name, ref bool v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (bool)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (short)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (int)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (long)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (decimal)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (string)pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (short[])pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (int[])pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (long[])pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = (string[])pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }


        public bool Get<P>(string name, ref P[] v, byte x = 0xff) where P : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<P>(string name, ref P v, byte x = 0xff) where P : IPersist, new()
        {
            throw new NotImplementedException();
        }
    }

}