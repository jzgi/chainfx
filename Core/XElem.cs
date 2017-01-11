using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// An XML element.
    ///
    public class XElem : ISource
    {
        readonly string name;

        Roll<XAttr> attributes;

        string text;

        XElem[] children;

        int count;

        public XElem(string name)
        {
            this.name = name;
        }

        public bool Flat { get; set; } = true;

        internal void AddAttribute(string name, string v)
        {
            if (attributes == null)
            {
                attributes = new Roll<XAttr>(8);
            }
            attributes.Add(new XAttr(name, v));
        }

        internal void AddChild(XElem e)
        {
            if (children == null)
            {
                children = new XElem[16];
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

        XElem FindChild(string name)
        {
            if (children != null)
            {
                for (int i = 0; i < count; i++)
                {
                    XElem elem = children[i];
                    if (elem.name.Equals(name))
                    {
                        return elem;
                    }
                }
            }
            return null;
        }

        public bool Get(string name, ref bool v)
        {
            if (Flat)
            {
                XAttr attr;
                if (attributes != null && attributes.TryGet(name, out attr))
                {
                    v = attr; return true;
                }
            }
            else
            {
                XElem e = FindChild(name);
                if (e != null)
                {
                    v = e.text.ToBool(); return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            XAttr attr;
            if (attributes.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte flags = 0) where D : IData, new()
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

        public bool Get<D>(string name, ref D[] v, byte flags = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }


    }
}