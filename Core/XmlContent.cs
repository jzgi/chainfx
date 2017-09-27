using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a UTF-8 encoded XML document. 
    /// </summary>
    public class XmlContent : DynamicContent, IDataOutput<XmlContent>
    {
        public XmlContent(bool octet, int capacity = 4096) : base(octet, capacity)
        {
        }

        public override string Type => "application/xml";


        void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '<')
                {
                    Add("&lt;");
                }
                else if (c == '>')
                {
                    Add("&gt;");
                }
                else if (c == '&')
                {
                    Add("&amp;");
                }
                else if (c == '"')
                {
                    Add("&quot;");
                }
                else
                {
                    Add(c);
                }
            }
        }

        public XmlContent ELEM(XElem e)
        {
            Add('<');
            Add(e.Tag);
            if (e.Attrs != null)
            {
                Roll<XAttr> attrs = e.Attrs;
                for (int i = 0; i < attrs.Count; i++)
                {
                    XAttr attr = attrs[i];
                    Add(' ');
                    Add(attr.Key);
                    Add('=');
                    Add('"');
                    AddEsc(attr.Value);
                    Add('"');
                }
            }
            Add('>');

            if (e.Text != null)
            {
                AddEsc(e.Text);
            }
            if (e.Count > 0)
            {
                for (int i = 0; i < e.Count; i++)
                {
                    ELEM(e.Child(i));
                }
            }
            Add("</");
            Add(e.Tag);
            Add('>');

            return this;
        }
        //
        // PUT
        //

        public XmlContent ELEM(string name, Action attrs, Action children)
        {
            Add('<');
            Add(name);

            attrs?.Invoke();

            Add('>');

            children?.Invoke();

            Add("</");
            Add(name);
            Add('>');

            return this;
        }

        public XmlContent ELEM(string name, bool v)
        {
            Add('<');
            Add(name);
            Add('>');
            Add(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }

        public XmlContent ELEM(string name, short v)
        {
            Add('<');
            Add(name);
            Add('>');
            Add(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }

        public XmlContent ELEM(string name, int v)
        {
            Add('<');
            Add(name);
            Add('>');
            Add(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }

        public XmlContent ELEM(string name, long v)
        {
            Add('<');
            Add(name);
            Add('>');
            Add(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }

        public XmlContent ELEM(string name, decimal v)
        {
            Add('<');
            Add(name);
            Add('>');
            Add(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }

        public XmlContent ELEM(string name, string v)
        {
            Add('<');
            Add(name);
            Add('>');
            AddEsc(v);
            Add('<');
            Add('/');
            Add(name);
            Add('>');
            return this;
        }


        public XmlContent PutNull(string name)
        {
            return this;
        }

        public XmlContent Put(string name, JNumber value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, IDataInput value)
        {
            return this;
        }

        public XmlContent Put(string name, bool value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, short value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, int value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, long value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, double value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, decimal value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, DateTime value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, string value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(value);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, ArraySegment<byte> value)
        {
            return this;
        }

        public XmlContent Put(string name, short[] value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, int[] value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, long[] value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, string[] value)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, Dictionary<string, string> value)
        {
            return this;
        }

        public XmlContent Put(string name, IData value, int proj = 0x00ff)
        {
            return this;
        }

        public XmlContent Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
        {
            return this;
        }
    }
}