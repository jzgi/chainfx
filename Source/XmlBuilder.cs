using System;
using System.Collections.Generic;

namespace ChainFX
{
    /// <summary>
    /// To generate a UTF-8 encoded XML document. 
    /// </summary>
    public class XmlBuilder : ContentBuilder, ISink
    {
        public XmlBuilder(bool bytely, int capacity) : base(bytely, capacity)
        {
        }

        public override string CType { get; set; } = "application/xml";

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

        public XmlBuilder ELEM(XElem elem)
        {
            Add('<');
            Add(elem.Tag);
            var attrs = elem.Attrs;
            if (attrs != null)
            {
                for (int i = 0; i < attrs.Count; i++)
                {
                    var ety = attrs.EntryAt(i);
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

        public XmlBuilder ELEM(string name, Action attrs, Action children)
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

        public XmlBuilder ELEM(string name, bool v)
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

        public XmlBuilder ELEM(string name, short v)
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

        public XmlBuilder ELEM(string name, int v)
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

        public XmlBuilder ELEM(string name, long v)
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

        public XmlBuilder ELEM(string name, decimal v)
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

        public XmlBuilder ELEM(string name, string v)
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

        //
        // SINK
        //

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

        public void Put(string name, bool v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');
            Add(v);
            Add('"');
        }

        public void Put(string name, char v)
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

        public void Put(string name, TimeSpan v)
        {
            throw new NotImplementedException();
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

        public void Put(string name, IList<byte> v)
        {
        }

        public void Put(string name, IList<short> v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Count; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
        }

        public void Put(string name, IList<int> v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Count; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
        }

        public void Put(string name, IList<long> v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Count; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
        }

        public void Put(string name, IList<string> v)
        {
            Add(' ');
            Add(name);
            Add('=');
            Add('"');

            for (int i = 0; i < v.Count; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }

            Add('"');
        }

        public void Put(string name, JObj v)
        {
        }

        public void Put(string name, JArr v)
        {
        }

        public void Put(string name, XElem v)
        {
            Add('<');
            Add(name);
            Add('<');
        }

        public void Put(string name, IData v, short msk = 0xff)
        {
        }

        public void Put<D>(string name, IList<D> v, short msk = 0xff) where D : IData
        {
        }

        public void PutFromSource(ISource s)
        {
        }
    }
}