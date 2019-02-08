namespace Greatbone
{
    /// <summary>
    /// An XML parser structure that deals with well-formed XML documents.
    /// </summary>
    public struct XmlParser : IParser<XElem>
    {
        static readonly ParserException ParserEx = new ParserException("error parsing xml");

        // bytes to parse
        readonly byte[] bytebuf;

        // chars to parse
        readonly string strbuf;

        readonly int length;

        // UTF-8 string builder
        readonly Text text;

        public XmlParser(byte[] bytebuf, int length)
        {
            this.bytebuf = bytebuf;
            this.strbuf = null;
            this.length = length;
            this.text = new Text(1024);
        }

        public XmlParser(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.length = strbuf.Length;
            this.text = new Text(1024);
        }

        int this[int index] => bytebuf?[index] ?? (int) strbuf[index];

        public XElem Parse()
        {
            int p = 0;

            // seek to a less-than (<)
            int b;
            while (IsWs(b = this[p]))
            {
                p++;
            } // skip ws
            if (b != '<') throw ParserEx;

            // the first char
            b = this[++p];

            if (b == '?') // skip the prolog line
            {
                while (this[++p] != '>')
                {
                }

                // seek to a <
                for (;;)
                {
                    b = this[++p];
                    if (IsWs(b)) continue; // skip ws
                    if (b == '<') break;
                    throw ParserEx;
                }
            }

            if (IsNameStartChar(b))
            {
                return ParseElem(ref p, b);
            }
            throw ParserEx;
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
            text.Clear();
            text.Accept(startchar);
            while (IsNameChar(b = this[++p]))
            {
                text.Accept(b); // to comprise start tag
            }
            string tag = text.ToString();
            XElem elem = new XElem(tag);

            // optionally parse attributes
            while (IsWs(b))
            {
                while (IsWs(b = this[++p]))
                {
                } // skip ws

                if (IsNameStartChar(b))
                {
                    // attribute name
                    text.Clear();
                    text.Accept(b);
                    while ((b = this[++p]) != '=')
                    {
                        text.Accept(b);
                    }
                    string name = text.ToString();

                    // attribute value
                    if (this[++p] != '"') throw ParserEx; // left quote
                    text.Clear();
                    while ((b = this[++p]) != '"') // till right quote
                    {
                        if (b == '&') // escape &lt; &gt; &amp; &quot;
                        {
                            int b1 = this[p + 1];
                            int b2 = this[p + 2];
                            int b3 = this[p + 3];
                            if (b1 == 'l' && b2 == 't' && b3 == ';')
                            {
                                b = '<';
                                p += 3;
                            }
                            else if (b1 == 'g' && b2 == 't' && b3 == ';')
                            {
                                b = '>';
                                p += 3;
                            }
                            else if (b1 == 'a' && b2 == 'm' && b3 == 'p' && this[p + 4] == ';')
                            {
                                b = '&';
                                p += 4;
                            }
                            else if (b1 == 'q' && b2 == 'u' && b3 == 'o' && this[p + 4] == 't' && this[p + 5] == ';')
                            {
                                b = '"';
                                p += 5;
                            }
                        }
                        text.Accept(b);
                    }
                    string value = text.ToString();

                    elem.AddAttr(name, value);

                    b = this[++p]; // step
                }
            } // end of attributes

            if (b == '>') // a start tag just finished, expecting the ending-tag
            {
                for (;;) // child nodes iteration
                {
                    while (IsWs(b = this[++p])) // skip ws
                    {
                    }

                    if (b == '<')
                    {
                        b = this[++p];
                        if (b == '/') // the ending tag
                        {
                            // consume
                            text.Clear();
                            while ((b = this[++p]) != '>')
                            {
                                text.Accept(b);
                            }
                            if (!text.Equals(tag)) throw ParserEx;

                            pos = p; // adjust current position
                            return elem;
                        }
                        else if (b == '!') // CDATA section
                        {
                            if (this[p + 1] == '[' && this[p + 2] == 'C' && this[p + 3] == 'D' && this[p + 4] == 'A' && this[p + 5] == 'T' && this[p + 6] == 'A' && this[p + 7] == '[')
                            {
                                text.Clear();
                                p += 7;
                                while ((b = this[++p]) != ']' || this[p + 1] != ']' || this[p + 2] != '>')
                                {
                                    text.Accept(b);
                                }
                                elem.Text = text.ToString();
                                p += 2; // skip ]>
                            }
                        }
                        else if (IsNameStartChar(b))
                        {
                            XElem child = ParseElem(ref p, b);
                            elem.Add(child);
                        }
                    }
                    else // text node
                    {
                        text.Clear();
                        while ((b = this[p]) != '<') // NOTE from the first char
                        {
                            if (b == '&') // escape &lt; &gt; &amp; &quot;
                            {
                                int b1 = this[p + 1];
                                int b2 = this[p + 2];
                                int b3 = this[p + 3];
                                if (b1 == 'l' && b2 == 't' && b3 == ';')
                                {
                                    b = '<';
                                    p += 3;
                                }
                                else if (b1 == 'g' && b2 == 't' && b3 == ';')
                                {
                                    b = '>';
                                    p += 3;
                                }
                                else if (b1 == 'a' && b2 == 'm' && b3 == 'p' && this[p + 4] == ';')
                                {
                                    b = '&';
                                    p += 4;
                                }
                                else if (b1 == 'q' && b2 == 'u' && b3 == 'o' && this[p + 4] == 't' && this[p + 5] == ';')
                                {
                                    b = '"';
                                    p += 5;
                                }
                            }
                            text.Accept(b);
                            ++p;
                        }
                        if (text.Count > 0)
                        {
                            elem.Text = text.ToString();
                        }
                        // NOTE decrease in position to behave as other child nodes
                        --p;
                    }
                } // child nodes iteration
            }
            if (b == '/' && this[++p] == '>') // empty-element
            {
                pos = p; // adjust current position
                return elem;
            }
            throw ParserEx;
        }
    }
}