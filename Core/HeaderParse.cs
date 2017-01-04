using System;

namespace Greatbone.Core
{
    ///
    /// Tp parse HTTP header field value;
    ///
    internal struct HeaderParse
    {
        static readonly ParseException ParseEx = new ParseException("error during header field parse");

        // field value
        readonly string value;

        // parsing position
        int pos;

        // string builder
        readonly Str str;

        internal HeaderParse(string v)
        {
            value = v;
            pos = 0;
            str = new Str(256);
        }

        public void Clear()
        {
            pos = 0;
            str.Clear();
        }

        public bool Match(string str)
        {
            int p = pos;
            if (value.IndexOf(str, p, StringComparison.Ordinal) != -1)
            {
                pos = p + str.Length; // move forward
                return true;
            }
            return false;
        }

        public string Parameter(string nameEq)
        {
            str.Clear();

            // parameter location
            int loc = value.IndexOf(nameEq, pos, StringComparison.Ordinal);
            if (loc == -1) loc = value.LastIndexOf(nameEq, pos, StringComparison.Ordinal);
            if (loc == -1) return null;

            int start = loc + nameEq.Length; // beginning of value
            if (start >= value.Length) throw ParseEx;

            bool quot = value[start] == '"';
            if (quot)
            {
                int p = start + 1;
                for (;;)
                {
                    if (p >= value.Length) throw ParseEx;
                    char c = value[p++];
                    if (c == '\\') // quoted-pair
                    {
                        p++;
                        str.Add(value[p]); // add the following char
                    }
                    else if (c == '"')
                    {
                        pos = p;
                        return str.ToString();
                    }
                    else
                    {
                        str.Add(c);
                    }
                }
            }
            else
            {
                int p = start;
                for (;;)
                {
                    if (p >= value.Length)
                    {
                        pos = p;
                        return str.ToString();
                    }
                    char c = value[p++];
                    if (c == ',' || c == '/' || c == ':' || c == ';') // a delimiter
                    {
                        pos = p;
                        return str.ToString();
                    }
                    else
                    {
                        str.Add(c);
                    }
                }
            }
        }
    }
}