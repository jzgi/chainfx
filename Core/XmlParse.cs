namespace Greatbone.Core
{
    ///
    /// An XML parsing structure that deals with well-formed XML documents.
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
            this.str = new Text(1024);
        }

        public XmlParse(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.offset = 0;
            this.length = strbuf.Length;
            this.str = new Text(1024);
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
                if (IsWs(b)) continue; // skip ws
                if (b == '<') break;
                throw ParseEx;
            }

            b = this[p++];
            if (b == '?') // skip the prolog line
            {
                while (this[p] != '>') { p++; }
                // seek to a <
                for (;;)
                {
                    b = this[p++];
                    if (IsWs(b)) continue; // skip ws
                    if (b == '<') break;
                    throw ParseEx;
                }
            }

            return ParseElem(ref p, b);
        }

        static bool IsWs(int c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        static bool IsNameStartChar(int c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        static bool IsNameChar(int c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || c == '-' || c >= '0' && c <= '9';
        }

        XElem ParseElem(ref int pos, int startchar)
        {
            int p = pos;
            int b;

            // parse element tag name
            str.Clear();
            str.Accept(startchar);
            while (IsNameChar(b = this[p++]))
            {
                str.Accept(b); // to comprise start tag
            }
            string tag = str.ToString();
            XElem elem = new XElem(tag);

            // optionally parse attributes
            while (IsWs(b))
            {
                while (IsWs(b = this[p++])) { } // skip ws

                if (IsNameStartChar(b))
                {
                    // attribute name
                    str.Clear();
                    str.Accept(b);
                    while ((b = this[p++]) != '=')
                    {
                        str.Accept(b);
                    }
                    string name = str.ToString();

                    // attribute value
                    if (this[p++] != '"') throw ParseEx; // left quote
                    str.Clear();
                    while ((b = this[p++]) != '"') // till right quote
                    {
                        str.Accept(b);
                    }
                    string value = str.ToString();

                    elem.AddAttr(name, value);

                    b = this[p++];
                }
            } // end of attributes

            if (b == '>') // a start tag just finished, expecting the ending-tag
            {
            NextChild:
                while (IsWs(b = this[p++])) { } // skip ws and forward

                if (b == '<')
                {
                    b = this[p++];
                    if (b == '/') // the ending tag
                    {
                        while ((b = this[p++]) != '>') { } // consume the tag
                        return elem;
                    }

                    if (IsNameStartChar(b))
                    {
                        elem.AddChild(ParseElem(ref p, b));

                        // if (this[p] == '<')
                        goto NextChild;
                    }
                }
                else // text node
                {
                    str.Clear();
                    while ((b = this[p++]) != '<')
                    {
                        str.Accept(b);
                    }
                    if (str.Count > 0)
                    {
                        elem.Text = str.ToString();
                    }
                }
            }
            else if (b == '/' && this[p++] == '>') // empty-element
            {
            }
            return elem;
        }
    }
}