namespace Greatbone.Core
{
    ///
    /// Parse JSON into object model from either bytes or string.
    ///
    public struct JsonParse
    {
        static readonly ParseException ParseEx = new ParseException("invalid json");

        // byte content to parse
        readonly byte[] bytebuf;

        // char content to parse
        readonly string strbuf;

        readonly int length;

        // UTF-8 string builder
        readonly Text str;

        public JsonParse(byte[] bytebuf, int length)
        {
            this.bytebuf = bytebuf;
            this.length = length;
            this.strbuf = null;
            this.str = new Text(256);
        }

        public JsonParse(string strbuf)
        {
            this.strbuf = strbuf;
            this.length = strbuf.Length;
            this.bytebuf = null;
            this.str = new Text(256);
        }

        int this[int index] => bytebuf?[index] ?? (int)strbuf[index];

        public IDataInput Parse()
        {
            int p = -1;
            for (;;)
            {
                if (p >= length - 1) throw ParseEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                if (b == '[') return ParseArr(ref p);
                throw ParseEx;
            }
        }

        JObj ParseObj(ref int pos)
        {
            JObj jobj = new JObj();
            int p = pos;
            for (;;)
            {
                for (;;)
                {
                    if (p >= length - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    if (b == '}') // close early empty
                    {
                        pos = p;
                        return jobj;
                    }
                    throw ParseEx;
                }

                str.Clear(); // parse name
                for (;;)
                {
                    if (p >= length - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == '"') break; // meet second quote
                    str.Add((char)b);
                }

                for (;;) // till a colon
                {
                    if (p >= length - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw ParseEx;
                }
                string name = str.ToString();

                // parse the value part
                for (;;)
                {
                    if (p >= length - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == '{')
                    {
                        JObj v = ParseObj(ref p);
                        jobj.Add(name, v);
                    }
                    else if (b == '[')
                    {
                        JArr v = ParseArr(ref p);
                        jobj.Add(name, v);
                    }
                    else if (b == '"')
                    {
                        string v = ParseString(ref p);
                        jobj.Add(name, v);
                    }
                    else if (b == 'n')
                    {
                        if (ParseNull(ref p)) jobj.AddNull(name);
                    }
                    else if (b == 't' || b == 'f')
                    {
                        bool v = ParseBool(ref p, b);
                        jobj.Add(name, v);
                    }
                    else if (b == '-' || b >= '0' && b <= '9')
                    {
                        JNumber v = ParseNumber(ref p, b);
                        jobj.Add(name, v);
                    }
                    else if (b == '&') // bytes extension
                    {
                        byte[] v = ParseBytes(p);
                        jobj.Add(name, v);
                    }
                    else throw ParseEx;
                    break;
                }

                // comma or end
                for (;;)
                {
                    if (p >= length - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}') // close normal
                    {
                        pos = p;
                        return jobj;
                    }
                    throw ParseEx;
                }
            }
        }

        JArr ParseArr(ref int pos)
        {
            JArr jarr = new JArr();
            int p = pos;
            for (;;)
            {
                if (p >= length - 1) throw ParseEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == ']') // close early empty
                {
                    pos = p;
                    return jarr;
                }
                if (b == '{')
                {
                    JObj v = ParseObj(ref p);
                    jarr.Add(new JMember(null, v));
                }
                else if (b == '[')
                {
                    JArr v = ParseArr(ref p);
                    jarr.Add(new JMember(null, v));
                }
                else if (b == '"')
                {
                    string v = ParseString(ref p);
                    jarr.Add(new JMember(v));
                }
                else if (b == 'n')
                {
                    if (ParseNull(ref p)) jarr.Add(new JMember());
                }
                else if (b == 't' || b == 'f')
                {
                    bool v = ParseBool(ref p, b);
                    jarr.Add(new JMember(null, v));
                }
                else if (b == '-' || b >= '0' && b <= '9')
                {
                    JNumber v = ParseNumber(ref p, b);
                    jarr.Add(new JMember(null, v));
                }
                else if (b == '&') // bytes extension
                {
                    byte[] v = ParseBytes(p);
                    jarr.Add(new JMember(null, v));
                }
                else throw ParseEx;

                // comma or return
                for (;;)
                {
                    if (p >= length - 1) throw ParseEx;
                    b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']') // close normal
                    {
                        pos = p;
                        return jarr;
                    }
                    throw ParseEx;
                }
            }
        }

        string ParseString(ref int pos)
        {
            str.Clear();
            int p = pos;
            bool esc = false;
            for (;;)
            {
                if (p >= length - 1) throw ParseEx;
                int b = this[++p];
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

        byte[] ParseBytes(int start)
        {
            return null;
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
                if (p >= length - 1) throw ParseEx;
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
            throw ParseEx;
        }
    }
}