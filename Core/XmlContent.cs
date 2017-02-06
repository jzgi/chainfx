using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// 
    /// To generate a UTF-8 encoded XML document. 
    /// 
    public class XmlContent : DynamicContent, IDataOutput<XmlContent>
    {
        public XmlContent() : base(true, true, 4096)
        {
        }

        public XmlContent(bool sendable, bool pooled, int capacity = 4096) : base(sendable, pooled, capacity)
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

        public XmlContent PutRaw(string name, string raw)
        {
            return this;
        }

        public XmlContent Put(string name, bool v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, short v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, int v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, long v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, double v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, decimal v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, JNumber v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, DateTime v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, NpgsqlPoint v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, char[] v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, string v, bool? anylen = null)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, byte[] v)
        {
            return this;
        }

        public XmlContent Put(string name, ArraySegment<byte> v)
        {
            return this;
        }

        public XmlContent Put(string name, short[] v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, int[] v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, long[] v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, string[] v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
            return this;
        }

        public XmlContent Put(string name, IData v, ushort proj = 0)
        {
            return this;
        }

        public XmlContent Put<D>(string name, D[] v, ushort proj = 0) where D : IData
        {
            return this;
        }

        public XmlContent Put<D>(string name, List<D> v, ushort proj = 0) where D : IData
        {
            return this;
        }

        public XmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public XmlContent Put(string name, Dictionary<string, string> v)
        {
            return this;
        }
    }
}