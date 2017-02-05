namespace Greatbone.Core
{
    ///
    /// A lightweight structure that parses well-formed XML documents.
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

        int this[int index] => bytebuf?[index] ?? (int)strbuf[index];

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
                while (this[p] != '>')
                {
                    p++;
                }
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

        XElem ParseElem(ref int pos, int firstchar)
        {
            str.Clear();

            str.Accept(firstchar);

            // parse tag name
            string name = null;
            int p = pos;
            for (;;)
            {
                int b = this[p++];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r')
                {
                    name = str.ToString();
                    continue; // skip ws
                }
                if (b == '>')
                {
                    name = str.ToString();
                    XElem e = new XElem(name);

                    break;
                }
                if (b == '/' && this[p++] == '>') // empty-element
                {
                    break;
                }

                str.Accept(b); // to comprise a tag name
            }

            // optional attributes



            // closing or start-close
            


            XElem elem = new XElem(name);

            for (;;)
            {
                int b = this[++p];
                if (p >= length) throw ParseEx;

                if (b == ' ') continue; // 

                if (b == '>') continue; // 

                if (b == '/') continue; // 
            }
        }

        void ParseAttrs(XElem elem) {

        }
    }
}