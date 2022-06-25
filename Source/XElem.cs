using System;
using System.Collections;

namespace CoChain
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

        public void Add(XElem el)
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
                    var tmp = children;
                    children = new XElem[len * 4];
                    Array.Copy(tmp, 0, children, 0, len);
                }
            }

            children[count++] = el;
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
                    var el = children[i];
                    if (el.tag.Equals(name))
                    {
                        current = i;
                        return el;
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

        public bool Get(string name, ref Guid v)
        {
            throw new NotImplementedException();
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
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref JArr v)
        {
            return false;
        }

        public bool Get(string name, ref XElem v)
        {
            return false;
        }

        public bool Get<D>(string name, ref D v, short msk = 0xff) where D : IData, new()
        {
            return false;
        }

        public bool Get<D>(string name, ref D[] v, short msk = 0xff) where D : IData, new()
        {
            return false;
        }

        public D ToObject<D>(short msk = 0xff) where D : IData, new()
        {
            return default;
        }

        public D[] ToArray<D>(short msk = 0xff) where D : IData, new()
        {
            return default;
        }

        public bool IsDataSet => false;

        public bool Next()
        {
            return false;
        }

        public void Write<C>(C cnt) where C : DynamicContent, ISink
        {
        }

        public IContent Dump()
        {
            var cnt = new XmlContent(true, 4096);
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