using System.Text;

namespace Greatbone.Core
{

    public struct JsonParse
    {
        static readonly FormatException FormatEx = new FormatException("JSON Format");

        readonly byte[] buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public JsonParse(byte[] buffer) : this(buffer, buffer.Length) { }

        public JsonParse(byte[] buffer, int count)
        {
            this.buffer = buffer;
            this.count = count;
            this.str = new Str();
        }

        public object Parse()
        {
            int p = -1;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                if (b == '[') return ParseArr(ref p);
                throw FormatEx;
            }
        }

        Obj ParseObj(ref int pos)
        {
            Obj obj = new Obj();
            int p = pos;
            for (;;)
            {
                for (;;)
                {
                    byte b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    throw FormatEx;
                }

                StringBuilder sb = new StringBuilder();
                for (;;)
                {
                    byte b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == '"') break; // meet second quote
                    else sb.Append((char)b);
                }

                for (;;) // till a colon
                {
                    byte b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw FormatEx;
                }

                // parse the value part
                for (;;)
                {
                    byte b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    string name = sb.ToString();
                    if (b == '{')
                    {
                        Obj v = ParseObj(ref p);
                        obj.Add(name, v);
                    }
                    else if (b == '[')
                    {
                        Arr v = ParseArr(ref p);
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
                    byte b = buffer[++p];
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

        Arr ParseArr(ref int pos)
        {
            Arr arr = new Arr(16);
            int p = pos;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{')
                {
                    Obj v = ParseObj(ref p);
                    arr.Add(new Member(v));
                }
                else if (b == '[')
                {
                    Arr v = ParseArr(ref p);
                    arr.Add(new Member(v));
                }
                else if (b == '"')
                {
                    string v = ParseString(ref p);
                    arr.Add(new Member(v));
                }
                else if (b == 'n')
                {
                    if (ParseNull(ref p)) arr.Add(new Member());
                }
                else if (b == 't' || b == 'f')
                {
                    bool v = ParseBool(ref p, b);
                    arr.Add(new Member(v));
                }
                else if (b == '-' || b >= '0' && b <= '9')
                {
                    Number v = ParseNumber(ref p, b);
                    arr.Add(new Member(v));
                }
                else if (b == '&') // bytes extension
                {
                    byte[] v = ParseBytes(p);
                    arr.Add(new Member(v));
                }
                else throw FormatEx;

                // comma or return
                for (;;)
                {
                    b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']')
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
                byte b = buffer[++p];
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

        Number ParseNumber(ref int pos, byte first)
        {
            Number num = new Number(first);
            int p = pos;
            for (;;)
            {
                byte b = buffer[p++];
                if (p >= count) throw FormatEx;
                if (b == '.')
                {
                    num.Point = true;
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

        bool ParseBool(ref int pos, byte first)
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