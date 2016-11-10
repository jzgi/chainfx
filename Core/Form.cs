using System;

namespace Greatbone.Core
{
    ///
    /// A form object model parsed from x-www-form-urlencoded.
    ///
    public class Form : ISource
    {
        const int InitialCapacity = 16;

        public static readonly Form Empty = new Form();

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
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
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
                v = pr;
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

        public bool Get(string name, ref Arr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            Pair pr;
            if (pairs.TryGet(name, out pr))
            {
                v = pr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Obj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] v)
        {
            throw new NotImplementedException();
        }


        public bool Get<D>(string name, ref D[] v, byte z = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte z = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }
    }
}