using System;
using System.Collections;

namespace Greatbone.Core
{
    /// <summary>
    /// An XML element.
    /// </summary>
    public class XElem : IDataInput, IComparable<XElem>, IEnumerable
    {
        readonly string tag;

        // attributes, can be null
        Map<string, string> attrs;

        // child elements. can be null
        XElem[] children;

        int count; // number of children

        int current;

        public XElem(string tag, Map<string, string> attrs = null)
        {
            this.tag = tag;
            this.attrs = attrs;
        }

        public string Tag => tag;

        public Map<string, string> Attrs => attrs;

        public string Attr(string attr) => attrs?[attr];

        /// <summary>
        /// The text node of this element, can be null.
        /// </summary>
        public string Text { get; internal set; }

        internal void AddAttr(string name, string value)
        {
            if (attrs == null)
            {
                attrs = new Map<string, string>(8);
            }
            attrs.Add(name, value);
        }

        public void Add(XElem elem)
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
            children[count++] = elem;
        }

        public void Add(string tag, string text)
        {
            var e = new XElem(tag)
            {
                Text = text
            };
            Add(e);
        }

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

        public XElem this[int index] => children?[index];

        public int Count => count;

        public void Sort()
        {
            Array.Sort(children, 0, count);
        }

        public bool Get(string name, ref bool v)
        {
            // try attribute
            if (attrs != null && attrs.TryGet(name, out var attr))
            {
                v = attr.ToBool();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = attr.ToShort();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = attr.ToInt();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = attr.ToLong();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = double.Parse(attr);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = decimal.Parse(attr);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (attrs.TryGet(name, out var attr))
            {
                v = attr.ToDateTime();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (attrs.TryGet(name, out var attr))
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

        public bool Get(string name, ref Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x1f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 0x1f) where D : IData, new()
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

        public IDataInput Let(out Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, byte proj = 0x1f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D[] v, byte proj = 0x1f) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public D ToObject<D>(byte proj = 0x1f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(byte proj = 0x1f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void Write<R>(IDataOutput<R> o) where R : IDataOutput<R>
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
            return v?.Text.ToInt() ?? 0;
        }

        public static implicit operator string(XElem v)
        {
            return v?.Text;
        }

        public int CompareTo(XElem other)
        {
            return string.CompareOrdinal(tag, other.tag);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}