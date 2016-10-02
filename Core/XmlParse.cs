using System.Text;

namespace Greatbone.Core
{

    public struct XmlParse
    {
        static readonly JsonException FormatEx = new JsonException("JSON Format");

        readonly byte[] buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public XmlParse(byte[] buffer) : this(buffer, buffer.Length) { }

        public XmlParse(byte[] buffer, int count)
        {
            this.buffer = buffer;
            this.count = count;
            this.str = new Str();
        }

        public Elem Parse()
        {
            int p = -1;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(ref p);
                throw FormatEx;
            }
        }

        Elem ParseObj(ref int pos)
        {
            Elem obj = new Elem();
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

                StringBuilder name = new StringBuilder();
                for (;;)
                {
                    byte b = buffer[++p];
                    if (p >= count) throw FormatEx;
                    if (b == '"') break; // meet second quote
                    else name.Append((char)b);
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
                    if (b == '{')
                    {
                        Elem v = ParseObj(ref p);
                        obj.Add(name.ToString(), v);
                    }

                    else if (b == '"')
                    {
                        string v = ParseString(p);
                        obj.Add(name.ToString(), v);
                    }
                    else if (b == 'n')
                    {
                        if (ParseNull(p)) obj.Add(name.ToString());
                    }
                    else if (b == 't' || b == 'f')
                    {
                        bool v = ParseBool(p);
                        obj.Add(name.ToString(), v);
                    }
                    else if (b >= '0' && b <= '9')
                    {
                        Number v = ParseNumber(p);
                        obj.Add(name.ToString(), v);
                    }
                    else if (b == '&') // bytes extension
                    {
                        byte[] v = ParseBytes(p);
                        obj.Add(name.ToString(), v);
                    }
                    else throw FormatEx;
                    break;
                }

                // comma or end
                for (;;)
                {
                    byte b = buffer[p++];
                    if (p >= count) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}') goto End;
                    throw FormatEx;
                }
            }
        End:
            return obj;
        }


        string ParseString(int start)
        {
            int p = start;
            for (;;)
            {
                byte b = buffer[p++];
                if (p >= count) throw FormatEx;
                if (b == '"') break;
                bool esc = false;
                char c = ' ';
                if (esc)
                {
                    c = c == '"' ? '"' : c == '\\' ? '\\' : c == 'b' ? '\b' : c == 'f' ? '\f' : c == 'n' ? '\n' : c == 'r' ? '\r' : c == 't' ? '\t' : c;
                    esc = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        esc = true;
                    }
                    str.Add(0);
                }

                throw FormatEx;
            }
            return null;
        }

        byte[] ParseBytes(int start)
        {
            return null;
        }

        bool ParseNull(int start)
        {
            int p = start;
            if (buffer[++p] == 'u' && buffer[++p] == 'l' && buffer[++p] == 'l')
            {
                return true;
            }
            return false;
        }

        Number ParseNumber(int start)
        {
            return default(Number);
        }

        bool ParseBool(int start)
        {
            int p = start;
            if (buffer[++p] == 'u' && buffer[++p] == 'l' && buffer[++p] == 'l')
            {
                return true;
            }
            return false;
        }
    }
}