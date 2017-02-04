namespace Greatbone.Core
{
    ///
    /// A simple XML parsing structure that deals with most common usages.
    ///
    public struct XmlParse
    {
        static readonly ParseException ParseEx = new ParseException("error parsing xml");

        // byte content to parse
        readonly byte[] bytebuf;

        // char content to parse
        readonly string strbuf;

        readonly int offset;

        readonly int length;

        // UTF-8 string builder
        readonly Text str;

        public XmlParse(byte[] bytebuf, int offset, int length)
        {
            this.bytebuf = bytebuf;
            this.strbuf = null;
            this.offset = offset;
            this.length = length;
            this.str = new Text(256);
        }

        public XmlParse(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.offset = 0;
            this.length = strbuf.Length;
            this.str = new Text(256);
        }

        int this[int index] => (bytebuf != null) ? bytebuf[index] : (int)strbuf[index];

        public XElem Parse()
        {
            int p = offset;

            // seek to a less-than (<)
            int b;
            for (;;)
            {
                b = this[p++];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '<') break;
                throw ParseEx;
            }

            b = this[p++];
            if (b == '?') // skip prolog
            {
                while (this[p] != '>') { p++; } // skip prolog
                // seek to a <
                for (;;)
                {
                    b = this[p++];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == '<') break;
                    throw ParseEx;
                }
            }

            return ParseElem(ref p, b);
        }

        static bool Ws(int c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        static bool Name(int c)
        {
            return c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || c == '-';
        }

        XElem ParseElem(ref int pos, int first)
        {
            str.Clear();

            str.Accept(first);

            // parse tag name
            string tag = null;
            int p = pos;
            for (;;)
            {
                int b = this[p++];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r')
                {
                    tag = str.ToString();
                    continue; // skip ws
                }
                if (b == '<') break;
                throw ParseEx;
            }

            for (;;)
            {
                int b = this[++p];
                if (p >= length) throw ParseEx;

                if (b == ' ') continue; // 

                if (b == '>') continue; // 

                if (b == '/') continue; // 
            }
        }
    }
}