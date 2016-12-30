using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// An XML element.
    ///
    public class XElem : Roll<XAttr>, ISource
    {
        const int InitialCapacity = 8;

        string text;

        XElem[] children;

        int count;

        public XElem() : base(InitialCapacity) { }

        internal void Add(string name, string v)
        {
            Add(new XAttr(name, v));
        }

        internal void AddChild(XElem e)
        {
            if (children == null)
            {
                children = new XElem[InitialCapacity];
            }
            else
            {
                int len = children.Length;
                if (count >= len)
                {
                    XElem[] temp = children;
                    children = new XElem[len * 4];
                    Array.Copy(temp, 0, children, 0, len);
                }
            }
            children[count++] = e;
        }

        public bool Get(string name, ref bool v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            XAttr attr;
            if (TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            XAttr attr;
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

        public bool Get<D>(string name, ref D v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
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

        public bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }
    }
}