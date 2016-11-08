namespace Greatbone.Core
{

    ///
    /// Parse JSON into object model from either bytes or string.
    ///
    public struct JParse
    {

        static readonly ParseException ParseEx = new ParseException("json");

        // byte buffer content to parse
        readonly byte[] bytebuf;

        readonly int count;

        string strbuf;

        // UTF-8 string builder
        readonly Str str;

        // whether json extension for byte array
        readonly bool jx;

        public JParse(string strbuf)
        {
            this.strbuf = strbuf;
            this.count = strbuf.Length;
            this.bytebuf = null;
            this.jx = false;
            this.str = new Str(256);
        }

        public JParse(byte[] bytebuf, int count, bool jx = false)
        {
            this.bytebuf = bytebuf;
            this.count = count;
            this.strbuf = null;
            this.jx = jx;
            this.str = new Str(256);
        }

        int this[int index] => (bytebuf != null) ? bytebuf[index] : (int)strbuf[index];

        public object Parse()
        {
            int p = -1;
            for (;;)
            {
                if (p >= count - 1) throw ParseEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                if (b == '[') return ParseArr(ref p);
                throw ParseEx;
            }
        }

        JObj ParseObj(ref int pos)
        {
            JObj obj = new JObj();
            int p = pos;
            for (;;)
            {
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    if (b == '}') // close early empty
                    {
                        pos = p;
                        return obj;
                    }
                    throw ParseEx;
                }

                str.Clear(); // parse name
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == '"') break; // meet second quote
                    else str.Add((char)b);
                }

                for (;;) // till a colon
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw ParseEx;
                }
                string name = str.ToString();

                // parse the value part
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == '{')
                    {
                        JObj v = ParseObj(ref p);
                        obj.Add(name, v);
                    }
                    else if (b == '[')
                    {
                        JArr v = ParseArr(ref p);
                        obj.Add(name, v);
                    }
                    else if (b == '"')
                    {
                        string v = ParseString(ref p);
                        obj.Add(name, v);
                    }
                    else if (b == 'n')
                    {
                        if (ParseNull(ref p)) obj.Add(name);
                    }
                    else if (b == 't' || b == 'f')
                    {
                        bool v = ParseBool(ref p, b);
                        obj.Add(name, v);
                    }
                    else if (b == '-' || b >= '0' && b <= '9')
                    {
                        Number v = ParseNumber(ref p, b);
                        obj.Add(name, v);
                    }
                    else if (b == '&') // bytes extension
                    {
                        byte[] v = ParseBytes(p);
                        obj.Add(name, v);
                    }
                    else throw ParseEx;
                    break;
                }

                // comma or end
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}') // close normal
                    {
                        pos = p;
                        return obj;
                    }
                    throw ParseEx;
                }
            }
        }

        JArr ParseArr(ref int pos)
        {
            JArr arr = new JArr(16);
            int p = pos;
            for (;;)
            {
                if (p >= count - 1) throw ParseEx;
                int b = this[++p];
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == ']') // close early empty
                {
                    pos = p;
                    return arr;
                }
                if (b == '{')
                {
                    JObj v = ParseObj(ref p);
                    arr.Add(new JMember(v));
                }
                else if (b == '[')
                {
                    JArr v = ParseArr(ref p);
                    arr.Add(new JMember(v));
                }
                else if (b == '"')
                {
                    string v = ParseString(ref p);
                    arr.Add(new JMember(v));
                }
                else if (b == 'n')
                {
                    if (ParseNull(ref p)) arr.Add(new JMember());
                }
                else if (b == 't' || b == 'f')
                {
                    bool v = ParseBool(ref p, b);
                    arr.Add(new JMember(v));
                }
                else if (b == '-' || b >= '0' && b <= '9')
                {
                    Number v = ParseNumber(ref p, b);
                    arr.Add(new JMember(v));
                }
                else if (b == '&') // bytes extension
                {
                    byte[] v = ParseBytes(p);
                    arr.Add(new JMember(v));
                }
                else throw ParseEx;

                // comma or return
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    b = this[++p];
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']') // close normal
                    {
                        pos = p;
                        return arr;
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
                if (p >= count - 1) throw ParseEx;
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

        Number ParseNumber(ref int pos, int first)
        {
            Number num = new Number(first);
            int p = pos;
            for (;;)
            {
                if (p >= count - 1) throw ParseEx;
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