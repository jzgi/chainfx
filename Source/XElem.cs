using System;
using System.Collections;

namespace Skyiah
{
    /// <summary>
    /// An XML element.
    /// </summary>
    public class XElem : ISource, IComparable<XElem>, IEnumerable
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
            if (attrs != null && attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToBool();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref char v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToChar();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref short v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToShort();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref int v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToInt();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref long v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToLong();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref double v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = double.Parse(attr);
                return true;
            }

            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = decimal.Parse(attr);
                return true;
            }

            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            if (attrs.TryGetValue(name, out var attr))
            {
                v = attr.ToDateTime();
                return true;
            }

            return false;
        }

        public bool Get(string name, ref string v)
        {
            if (attrs.TryGetValue(name, out var attr))
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

        public bool Get(string name, ref byte[] v)
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

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }


        public D ToObject<D>(byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(byte proj = 0x0f) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool IsDataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            var cnt = new XmlContent(4096);
            cnt.ELEM(this);
            return cnt;
        }

        public int CompareTo(XElem other)
        {
            return string.CompareOrdinal(tag, other.tag);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static implicit operator string(XElem v)
        {
            return v?.Text;
        }

        public static implicit operator int(XElem v)
        {
            return v?.Text.ToInt() ?? 0;
        }
    }
}