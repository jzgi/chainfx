namespace ChainFx
{
    ///
    /// Parse JSON into object model from either bytes or string.
    ///
    public readonly struct JsonParser : IParser<ISource>
    {
        static readonly ParserException ParserEx = new ParserException("error parsing json");

        // byte content to parse
        readonly byte[] bytebuf;

        // char content to parse
        readonly string strbuf;

        readonly int length;

        // UTF-8 string builder
        readonly Text text;

        public JsonParser(byte[] bytebuf, int length)
        {
            this.bytebuf = bytebuf;
            this.length = length;
            this.strbuf = null;
            this.text = new Text(512);
        }

        public JsonParser(string strbuf)
        {
            this.strbuf = strbuf;
            this.length = strbuf.Length;
            this.bytebuf = null;
            this.text = new Text(512);
        }

        int this[int index] => bytebuf?[index] ?? (int) strbuf[index];

        public ISource Parse()
        {
            int p = -1;
            for (;;)
            {
                if (p >= length - 1) throw ParserEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                if (b == '[') return ParseArr(ref p);
                throw ParserEx;
            }
        }

        JObj ParseObj(ref int pos)
        {
            JObj jo = new JObj();
            int p = pos;
            for (;;)
            {
                for (;;)
                {
                    if (p >= length - 1) throw ParserEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    if (b == '}') // close early empty
                    {
                        pos = p;
                        return jo;
                    }
                    throw ParserEx;
                }

                text.Clear(); // parse name
                for (;;)
                {
                    if (p >= length - 1) throw ParserEx;
                    int b = this[++p];
                    if (b == '"') break; // meet second quote
                    text.Add((char) b);
                }

                for (;;) // till a colon
                {
                    if (p >= length - 1) throw ParserEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw ParserEx;
                }
                string name = text.ToString();

                // parse the value part
                for (;;)
                {
                    if (p >= length - 1) throw ParserEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == '{')
                    {
                        JObj v = ParseObj(ref p);
                        jo.Add(name, v);
                    }
                    else if (b == '[')
                    {
                        JArr v = ParseArr(ref p);
                        jo.Add(name, v);
                    }
                    else if (b == '"')
                    {
                        string v = ParseString(ref p);
                        jo.Add(name, v);
                    }
                    else if (b == 'n')
                    {
                        if (ParseNull(ref p)) jo.Add(name);
                    }
                    else if (b == 't' || b == 'f')
                    {
                        bool v = ParseBool(ref p, b);
                        jo.Add(name, v);
                    }
                    else if (b == '-' || b >= '0' && b <= '9')
                    {
                        JNumber v = ParseNumber(ref p, b);
                        jo.Add(name, v);
                    }
                    else throw ParserEx;
                    break;
                }

                // comma or end
                for (;;)
                {
                    if (p >= length - 1) throw ParserEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}') // close normal
                    {
                        pos = p;
                        return jo;
                    }
                    throw ParserEx;
                }
            }
        }

        JArr ParseArr(ref int pos)
        {
            JArr ja = new JArr();
            int p = pos;
            for (;;)
            {
                if (p >= length - 1) throw ParserEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == ']') // close early empty
                {
                    pos = p;
                    return ja;
                }
                if (b == '{')
                {
                    JObj v = ParseObj(ref p);
                    ja.Add(new JMbr(v));
                }
                else if (b == '[')
                {
                    JArr v = ParseArr(ref p);
                    ja.Add(new JMbr(v));
                }
                else if (b == '"')
                {
                    string v = ParseString(ref p);
                    ja.Add(new JMbr(v));
                }
                else if (b == 'n')
                {
                    if (ParseNull(ref p)) ja.Add(new JMbr(JType.Null));
                }
                else if (b == 't' || b == 'f')
                {
                    bool v = ParseBool(ref p, b);
                    ja.Add(new JMbr(v));
                }
                else if (b == '-' || b >= '0' && b <= '9')
                {
                    JNumber v = ParseNumber(ref p, b);
                    ja.Add(new JMbr(v));
                }
                else throw ParserEx;

                // comma or return
                for (;;)
                {
                    if (p >= length - 1) throw ParserEx;
                    b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']') // close normal
                    {
                        pos = p;
                        return ja;
                    }
                    throw ParserEx;
                }
            }
        }

        string ParseString(ref int pos)
        {
            text.Clear();
            int p = pos;
            bool esc = false;
            for (;;)
            {
                if (p >= length - 1) throw ParserEx;
                int b = this[++p];
                if (esc)
                {
                    text.Add(b == '"' ? '"' :
                        b == '\\' ? '\\' :
                        b == 'b' ? '\b' :
                        b == 'f' ? '\f' :
                        b == 'n' ? '\n' :
                        b == 'r' ? '\r' :
                        b == 't' ? '\t' : (char) 0);
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
                        return text.ToString();
                    }
                    else
                    {
                        text.Accept(b);
                    }
                }
            }
        }

        bool ParseNull(ref int pos)
        {
            int p = pos;
            if (this[++p] == 'u' && this[++p] == 'l' && this[++p] == 'l')
            {
                pos = p;
                return true;
            }
            return false;
        }

        JNumber ParseNumber(ref int pos, int first)
        {
            JNumber num = new JNumber(first);
            int p = pos;
            for (;;)
            {
                if (p >= length - 1) throw ParserEx;
                int b = this[++p];
                if (b == '.')
                {
                    num.Pt = true;
                }
                else if (b >= '0' && b <= '9')
                {
                    num.Add(b);
                }
                else
                {
                    pos = p - 1;
                    return num;
                }
            }
        }

        bool ParseBool(ref int pos, int first)
        {
            int p = pos;
            if (first == 't')
            {
                if (this[++p] == 'r' && this[++p] == 'u' && this[++p] == 'e')
                {
                    pos = p;
                    return true;
                }
            }
            else if (first == 'f')
            {
                if (this[++p] == 'a' && this[++p] == 'l' && this[++p] == 's' && this[++p] == 'e')
                {
                    pos = p;
                    return false;
                }
            }
            throw ParserEx;
        }
    }
}