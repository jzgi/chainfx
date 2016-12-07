using System;

namespace Greatbone.Core
{
    ///
    /// An XML element.
    ///
    public class Elem : Roll<Attr>, ISource
    {
        const int InitialCapacity = 8;

        string text;

        Elem[] children;

        int count;

        public Elem() : base(InitialCapacity) { }

        internal void Add(string name, string v)
        {
            Add(new Attr(name, v));
        }

        internal void AddChild(Elem e)
        {
            if (children == null)
            {
                children = new Elem[InitialCapacity];
            }
            else
            {
                int len = children.Length;
                if (count >= len)
                {
                    Elem[] temp = children;
                    children = new Elem[len * 4];
                    Array.Copy(temp, 0, children, 0, len);
                }
            }
            children[count++] = e;
        }

        public bool Get(string name, ref bool v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            Attr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte z = 0) where D : IDat, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Obj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Arr v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte z = 0) where D : IDat, new()
        {
            throw new NotImplementedException();
        }
    }
}