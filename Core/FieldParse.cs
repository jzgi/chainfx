using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// Tp parse HTTP header field value;
    /// </summary>
    ///
    struct FieldParse
    {
        static readonly ParseException FormatEx = new ParseException("wrong field value Format");

        // field value
        readonly string fvalue;

        // parsing position
        int pos;

        // string builder
        readonly Str str;

        internal FieldParse(string v)
        {
            fvalue = v;
            pos = 0;
            str = new Str(256);
        }

        public void Clear()
        {
            pos = 0;
            str.Clear();
        }

        public bool Match(String str)
        {
            int p = pos;
            if (fvalue.IndexOf(str, p) != -1)
            {
                pos = p += str.Length; // move forward
                return true;
            }
            return false;
        }

        public String Parameter(String nameEq)
        {
            str.Clear();

            // parameter location
            int loc = fvalue.IndexOf(nameEq, pos);
            if (loc == -1) loc = fvalue.LastIndexOf(nameEq, pos);
            if (loc == -1) return null;

            int start = loc + nameEq.Length; // beginning of value
            if (start >= fvalue.Length) throw FormatEx;

            bool quot = fvalue[start] == '"';
            if (quot)
            {
                int p = start + 1;
                for (;;)
                {
                    if (p >= fvalue.Length) throw FormatEx;
                    char c = fvalue[p++];
                    if (c == '\\') // quoted-pair
                    {
                        p++;
                        str.AddChar(fvalue[p]); // add the following char
                    }
                    else if (c == '"')
                    {
                        pos = p;
                        return str.ToString();
                    }
                    else
                    {
                        str.AddChar(c);
                    }
                }
            }
            else
            {
                int p = start;
                for (;;)
                {
                    if (p >= fvalue.Length)
                    {
                        pos = p;
                        return str.ToString();
                    }
                    char c = fvalue[p++];
                    if (c == ',' || c == '/' || c == ':' || c == ';') // a delimiter
                    {
                        pos = p;
                        return str.ToString();
                    }
                    else
                    {
                        str.AddChar(c);
                    }
                }
            }
        }

    }

}