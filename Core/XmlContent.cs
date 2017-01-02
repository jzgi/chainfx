using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// 
    /// To generate a UTF-8 encoded XML document. 
    /// 
    public class XmlContent : DynamicContent, ISink<XmlContent>
    {
        const int InitialCapacity = 4 * 1024;

        bool start;

        public XmlContent(bool sendable, bool pooled, int capacity = InitialCapacity) : base(sendable, pooled, capacity)
        {
        }

        public override string MimeType => "application/xml";


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

        public void PutElem(string name, Action a)
        {
            if (start) { Add('>'); start = false; }

            Add('<');
            Add(name);
            start = true;

            if (a != null) a();

            if (start) { Add('>'); start = false; }
            Add("</");
            Add(name);
            Add('>');
        }

        public XmlContent PutNull(string name)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public XmlContent Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public XmlContent Put<D>(string name, D v, byte bits = 0) where D : IData
        {
            throw new NotImplementedException();
        }

        public XmlContent Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public XmlContent Put(string name, JArr v)
        {
            throw new NotImplementedException();
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

        public XmlContent Put<D>(string name, D[] v, byte bits = 0) where D : IData
        {
            throw new NotImplementedException();
        }
    }
}