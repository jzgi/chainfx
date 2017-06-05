using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// An XML element tag.
    ///
    public class XElem : IDataInput, IComparable<XElem>
    {
        readonly string tag;

        // attributes, can be null
        Roll<XAttr> attrs;

        // text node, can be null
        string text;

        // child elements. can be null
        XElem[] children;

        int count; // number of children

        public XElem(string tag)
        {
            this.tag = tag;
        }

        public string Tag => tag;

        public Roll<XAttr> Attrs => attrs;

        public XAttr Attr(string attr) => attrs[attr];

        internal void AddAttr(string name, string v)
        {
            if (attrs == null)
            {
                attrs = new Roll<XAttr>(8);
            }
            attrs.Add(new XAttr(name, v));
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

        public void AddChild(string tag, string text)
        {
            var e = new XElem(tag);
            e.Text = text;
            AddChild(e);
        }

        public string Text
        {
            get => text;
            internal set => text = value;
        }

        int current;

        public XElem Child(string name)
        {
            if (children != null)
            {
                for (int i = current; i < count; i++)
                {
                    XElem elem = children[i];
                    if (elem.tag.Equals(name))
                    {
                        current = i;
                        return elem;
                    }
                }
                for (int i = 0; i < current; i++)
                {
                    XElem elem = children[i];
                    if (elem.tag.Equals(name))
                    {
                        current = i;
                        return elem;
                    }
                }
            }
            return null;
        }

        public XElem Child(int index) => children?[index];

        public int Count => count;


        public void Sort()
        {
            Array.Sort(children, 0, count);
        }

        public bool Get(string name, ref bool v)
        {
            // try attribute
            XAttr attr;
            if (attrs != null && attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }


        public bool Get(string name, ref short v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
            {
                v = attr;
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            XAttr attr;
            if (attrs.TryGet(name, out attr))
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

        public bool Get(string name, ref Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        //
        // LET
        //

        public IDataInput Let(out bool v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out double v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out decimal v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out DateTime v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out short[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out int[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out long[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out string[] v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let(out Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D[] v, ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public D ToData<D>(ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToDatas<D>(ushort proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            throw new NotImplementedException();
        }

        public DynamicContent Dump()
        {
            return new XmlContent(true).ELEM(this);
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator int(XElem v)
        {
            return v?.text.ToInt() ?? 0;
        }

        public static implicit operator string(XElem v)
        {
            return v?.text;
        }

        public int CompareTo(XElem other)
        {
            return String.CompareOrdinal(tag, other.tag);
        }
    }
}