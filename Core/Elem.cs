using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// An XML element.
    /// </summary>
    ///
    public class Elem : ISource
    {
        const int InitialCapacity = 8;

        Roll<Attr> attrs;

        string text;

        Elem[] children;

        int count;

        public Elem(int capacity = InitialCapacity)
        {
        }

        internal void AddAttr(string name, string v)
        {
            if (attrs == null)
            {
                attrs = new Roll<Attr>(8);
            }
            Attr pair = new Attr(name, v);
            attrs.Add(pair);
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
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            if (attrs != null)
            {
                Attr attr;
                if (attrs.TryGet(name, out attr))
                {
                    v = attr;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            throw new NotImplementedException();
        }

        public bool Get<V>(string name, ref V v, byte z = 0) where V : IBean, new()
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

        public bool Get<V>(string name, ref V[] v, byte z = 0) where V : IBean, new()
        {
            throw new NotImplementedException();
        }

        public int Count => attrs.Count;

        public Attr this[int index] => attrs[index];

        public Attr this[string name] => attrs[name];


        //
        // SOURCE
        //

    }

}