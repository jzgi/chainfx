using System;
using System.Collections.Generic;

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

        public XmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public XmlContent PutRaw(string name, string raw)
        {
            return this;
        }

        public void Begin(string label)
        {
        }

        public void End()
        {
        }

        public XmlContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, short v, Opt<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, int v, Opt<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, long v, Opt<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, string v, Opt<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
            return this;
        }

        public XmlContent Put(string name, ArraySegment<byte> v, string Label = null, string size = null, string Ratio = null, bool required = false)
        {
            return this;
        }

        public XmlContent Put(string name, short[] v, Opt<short> Opt = null, string Label = null, string Help = null, bool @readonly = false, bool required = false)
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

        public XmlContent Put(string name, int[] v, Opt<int> Opt = null, string Label = null, string Help = null, bool @readonly = false, bool required = false)
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

        public XmlContent Put(string name, long[] v, Opt<long> Opt = null, string Label = null, string Help = null, bool @readonly = false, bool required = false)
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

        public XmlContent Put(string name, string[] v, Opt<string> Opt = null, string Label = null, string Help = null, bool @readonly = false, bool required = false)
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

        public XmlContent Put(string name, Dictionary<string, string> v, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public XmlContent Put(string name, IData v, int proj = 0, string Label = null, string Help = null, bool @readonly = false, bool required = false)
        {
            return this;
        }

        public XmlContent Put<D>(string name, D[] v, int proj = 0, string Label = null, string Help = null, bool @readonly = false, bool required = false) where D : IData
        {
            return this;
        }

        public XmlContent Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Help = null, bool @readonly = false, bool required = false) where D : IData
        {
            return this;
        }
    }
}