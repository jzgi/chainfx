using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a UTF-8 encoded XML document. 
    /// </summary>
    public class XmlContent : DynamicContent, ISink
    {
        public XmlContent(bool bin, int capacity = 4096) : base(bin, capacity)
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

        public XmlContent ELEM(XElem elem)
        {
            Add('<');
            Add(elem.Tag);
            Map<string, string> attrs = elem.Attrs;
            if (attrs != null)
            {
                for (int i = 0; i < attrs.Count; i++)
                {
                    var ety = attrs.At(i);
                    Add(' ');
                    Add(ety.Key);
                    Add('=');
                    Add('"');
                    AddEsc(ety.Value);
                    Add('"');
                }
            }
            Add('>');

            if (elem.Text != null)
            {
                AddEsc(elem.Text);
            }
            if (elem.Count > 0)
            {
                for (int i = 0; i < elem.Count; i++)
                {
                    ELEM(elem[i]);
                }
            }
            Add("</");
            Add(elem.Tag);
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


        public void PutOpen()
        {
        }

        public void PutClose()
        {
        }

        public void PutStart()
        {
        }

        public void PutEnd()
        {
        }

        public void PutNull(string name)
        {
        }

        public void Put(string name, JNumber v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, ISource v)
        {
        }

        public void Put(string name, bool v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, short v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, int v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, long v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, double v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, decimal v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, DateTime v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, string v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, ArraySegment<byte> v)
        {
        }

        public void Put(string name, short[] v)
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
        }

        public void Put(string name, int[] v)
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
        }

        public void Put(string name, long[] v)
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
        }

        public void Put(string name, string[] v)
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
        }

        public void Put(string name, Map<string, string> v)
        {
        }

        public void Put(string name, IData v, byte proj = 0x0f)
        {
        }

        public void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData
        {
        }
    }
}