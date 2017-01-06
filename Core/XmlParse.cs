namespace Greatbone.Core
{
    ///
    /// A simple XML parsing structure that deals with most common usages.
    ///
    public struct XmlParse
    {
        static readonly ParseException ParseEx = new ParseException("error during xml parse");

        // byte content to parse
        readonly byte[] bytebuf;

        // char content to parse
        readonly string strbuf;

        readonly int length;

        // UTF-8 string builder
        readonly Str str;

        public XmlParse(byte[] bytebuf, int length)
        {
            this.bytebuf = bytebuf;
            this.strbuf = null;
            this.length = length;
            this.str = new Str(256);
        }

        public XmlParse(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.length = strbuf.Length;
            this.str = new Str(256);
        }

        int this[int index] => (bytebuf != null) ? bytebuf[index] : (int)strbuf[index];

        public object Parse()
        {
            int p = -1;
            for (;;)
            {
                byte b = bytebuf[++p];
                if (p >= length) throw ParseEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                throw ParseEx;
            }
        }

        string ParseString(ref int pos)
        {
            str.Clear();
            int p = pos;
            bool esc = false;
            for (;;)
            {
                byte b = bytebuf[++p];
                if (p >= length) throw ParseEx;
                if (esc)
                {
                    str.Add(b == '"' ? '"' : b == '\\' ? '\\' : b == 'b' ? '\b' : b == 'f' ? '\f' : b == 'n' ? '\n' : b == 'r' ? '\r' : b == 't' ? '\t' : (char)0);
                    esc = !esc;
                }
                else
                {
                    if (b == '\\')
                    {
                        esc = !esc;
                    }
                    else if (b == '"')
                    {
                        pos = p;
                        return str.ToString();
                    }
                    else
                    {
                        str.Accept(b);
                    }
                }
            }
        }
    }
}