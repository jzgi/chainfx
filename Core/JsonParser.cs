using System.Text;

namespace Greatbone.Core
{

    public class JsonParser
    {
        static readonly JsonException FormatEx = new JsonException("JSON Format");

        readonly byte[] array;

        readonly int limit;

        int pos;

        // UTF-8 string builder
        Str str;

        public JsonParser(byte[] array) : this(array, array.Length) { }

        public JsonParser(byte[] array, int limit)
        {
            this.array = array;
            this.limit = limit;
            pos = -1;
        }

        public object Parse()
        {
            int p = pos;
            for (;;)
            {
                byte b = array[++p];
                if (p >= limit) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{') return ParseObj(p);
                if (b == '[') return ParseArr(p);
                throw FormatEx;
            }
        }

        Obj ParseObj(int start)
        {
            Obj obj = new Obj();
            int p = start;
            for (;;)
            {
                for (;;)
                {
                    byte b = array[++p];
                    if (p >= limit) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == '"') break; // meet first quote
                    throw FormatEx;
                }

                StringBuilder name = new StringBuilder();
                for (;;)
                {
                    byte b = array[++p];
                    if (p >= limit) throw FormatEx;
                    if (b == '"') break; // meet second quote
                    else name.Append((char)b);
                }

                for (;;) // till a colon
                {
                    byte b = array[++p];
                    if (p >= limit) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ':') break;
                    throw FormatEx;
                }

                // parse the value part
                for (;;)
                {
                    byte b = array[++p];
                    if (p >= limit) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == '{')
                    {
                        Obj v = ParseObj(p);
                        obj.Add(name.ToString(), v);
                    }
                    else if (b == '[')
                    {
                        Arr v = ParseArr(p);
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
                    byte b = array[p++];
                    if (p >= limit) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue;
                    if (b == ',') break;
                    if (b == '}') goto End;
                    throw FormatEx;
                }
            }
        End:
            return obj;
        }

        Arr ParseArr(int start)
        {
            Arr arr = new Arr(16);
            int p = start;
            for (;;)
            {
                byte b = array[++p];
                if (p >= limit) throw FormatEx;
                if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                if (b == '{')
                {
                    Obj v = ParseObj(p);
                    arr.Add(new Member(v));
                }
                else if (b == '[')
                {
                    Arr v = ParseArr(p);
                    arr.Add(new Member(v));
                }
                else if (b == '"')
                {
                    string v = ParseString(p);
                    arr.Add(new Member(v));
                }
                else if (b == 'n')
                {
                    if (ParseNull(p)) arr.Add(new Member());
                }
                else if (b == 't' || b == 'f')
                {
                    bool v = ParseBool(p);
                    arr.Add(new Member(v));
                }
                else if (b >= '0' && b <= '9')
                {
                    Number v = ParseNumber(p);
                    arr.Add(new Member(v));
                }
                else if (b == '&') // bytes extension
                {
                    byte[] v = ParseBytes(p);
                    arr.Add(new Member(v));
                }
                else throw FormatEx;

                // comma or end
                for (;;)
                {
                    b = array[p++];
                    if (p >= limit) throw FormatEx;
                    if (b == ' ' || b == '\t' || b == '\n' || b == '\r') continue; // skip ws
                    if (b == ',') break;
                    if (b == ']') goto End;
                    throw FormatEx;
                }
            }
        End:
            return null;
        }

        string ParseString(int start)
        {
            return null;
        }

        byte[] ParseBytes(int start)
        {
            return null;
        }

        bool ParseNull(int start)
        {
            return false;
        }

        Number ParseNumber(int start)
        {
            return default(Number);
        }

        bool ParseBool(int start)
        {
            return false;
        }
    }
}