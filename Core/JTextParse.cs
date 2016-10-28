namespace Greatbone.Core
{

    public struct JTextParse
    {
        static readonly ParseException FormatEx = new ParseException("wrong text json Format");

        readonly string buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public JTextParse(string str) : this(str, str.Length) { }

        public JTextParse(string str, int count)
        {
            this.buffer = str;
            this.count = count;
            this.str = new Str(256);
        }

        public object Parse()
        {
            int p = -1;
            for (;;)
            {
                char b = buffer[++p];
                if (p >= count) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                if (b == '[') return ParseArr(ref p);
                throw FormatEx;
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
                    char b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    if (b == '}') // emoty close
                    {
                        pos = p;
                        return obj; 
                    }
                    throw FormatEx;
                }

                str.Clear(); // parse name                
                for (;;)
                {
                    char b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == '"') break; // meet second quote
                    else str.Add((char)b);
                }

                for (;;) // till a colon
                {
                    char b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw FormatEx;
                }
                string name = str.ToString();

                // parse the value part
                for (;;)
                {
                    char b = buffer[++p];
                    if (p >= count) throw FormatEx;
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
                    else throw FormatEx;
                    break;
                }

                // comma or end
                for (;;)
                {
                    char b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}')
                    {
                        pos = p;
                        return obj;
                    }
                    throw FormatEx;
                }
            }

        }

        JArr ParseArr(ref int pos)
        {
            JArr arr = new JArr(16);
            int p = pos;
            for (;;)
            {
                char b = buffer[++p];
                if (p >= count) throw FormatEx;
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
                else throw FormatEx;

                // comma or return
                for (;;)
                {
                    b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']') // close normal
                    {
                        pos = p;
                        return arr;
                    }
                    throw FormatEx;
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
                char b = buffer[++p];
                if (p >= count) throw FormatEx;
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
                        str.Add(b);
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
            if (buffer[++p] == 'u' && buffer[++p] == 'l' && buffer[++p] == 'l')
            {
                pos = p;
                return true;
            }
            return false;
        }

        Number ParseNumber(ref int pos, char first)
        {
            Number num = new Number((byte)first);
            int p = pos;
            for (;;)
            {
                if (p >= count) throw FormatEx;
                char b = buffer[++p];
                if (b == '.')
                {
                    num.Pt = true;
                }
                else if (b >= '0' && b <= '9')
                {
                    num.Add((byte)b);
                }
                else
                {
                    pos = p - 1;
                    return num;
                }
            }

        }

        bool ParseBool(ref int pos, char first)
        {
            int p = pos;
            if (first == 't')
            {
                if (buffer[++p] == 'r' && buffer[++p] == 'u' && buffer[++p] == 'e')
                {
                    pos = p;
                    return true;
                }
            }
            else if (first == 'f')
            {
                if (buffer[++p] == 'a' && buffer[++p] == 'l' && buffer[++p] == 's' && buffer[++p] == 'e')
                {
                    pos = p;
                    return false;
                }
            }
            throw FormatEx;
        }
    }
}