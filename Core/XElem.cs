using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// An XML element.
    ///
    public class XElem : IModel, ISource
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

        public XElem this[string name] => null;

        public bool Flat { get; set; } = true;

        public bool One
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        public bool Get<D>(string name, ref List<D> v, byte flags = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(byte flags = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
            throw new NotImplementedException();
        }

        public C Dump<C>() where C : IContent, ISink<C>, new()
        {
            C cont = new C();
            Dump(cont);
            return cont;
        }

        public static implicit operator string(XElem v)
        {
            return v.text;
        }
    }
}