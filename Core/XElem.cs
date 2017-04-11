using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// An XML element tag.
    ///
    public class XElem : IDataInput
    {
        readonly string tag;

        // attributes, can be null
        Roll<XAttr> attrs;

        // child text node, can be null
        string text;

        // child element nodes. can be null
        XElem[] elems;

        int count; // number of subs

        public XElem(string tag)
        {
            this.tag = tag;
        }

        public XAttr Attr(string attr) => attrs[attr];

        public bool DataSet => false;

        internal void AddAttr(string name, string v)
        {
            if (attrs == null)
            {
                attrs = new Roll<XAttr>(8);
            }
            attrs.Add(new XAttr(name, v));
        }

        internal void AddSub(XElem e)
        {
            if (elems == null)
            {
                elems = new XElem[16];
            }
            else
            {
                int len = elems.Length;
                if (count >= len)
                {
                    XElem[] temp = elems;
                    elems = new XElem[len * 4];
                    Array.Copy(temp, 0, elems, 0, len);
                }
            }
            elems[count++] = e;
        }


        public string Text
        {
            get { return text; }
            internal set { text = value; }
        }

        int current;

        public XElem Child(string name)
        {
            if (elems != null)
            {
                for (int i = current; i < count; i++)
                {
                    XElem elem = elems[i];
                    if (elem.tag.Equals(name))
                    {
                        current = i;
                        return elem;
                    }
                }
                for (int i = 0; i < current; i++)
                {
                    XElem elem = elems[i];
                    if (elem.tag.Equals(name))
                    {
                        current = i;
                        return elem;
                    }
                }
            }
            return null;
        }

        public bool Sub(string name, ref bool v)
        {
            // try sub
            XElem e = Child(name);
            if (e != null)
            {
                v = e.text.ToBool();
                return true;
            }
            return false;
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

        public bool Get<D>(string name, ref D v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref List<D> v, int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            var cont = new XmlContent(true, true);
            cont.Put(null, this);
            return cont;
        }

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(int proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public List<D> ToList<D>(int proj = 0) where D : IData, new()
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
    }
}