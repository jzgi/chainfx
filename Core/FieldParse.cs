using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A HTTP header field value tokenizer.
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
            int p;
            if (fvalue.IndexOf(str, pos) != -1)
            {
                pos += str.Length; // move forward
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
            int p = quot ? ++start : start;
            for (;;)
            {
                if (p >= fvalue.Length) throw FormatEx;

                char c = fvalue[p++];
                if (c == '/') // quoted-pair
                {
                    if (!quot) throw FormatEx;
                    p++;
                    str.Add(fvalue[p]); // add the following char
                }
                else if (c == '"')
                {
                    if (quot)
                    {
                        return str.ToString();
                    }
                    else
                    {
                        str.Add(c);
                    }
                }
                else if (c ==' ') // TODO
                {
                    str.Add(c);
                }
            }
        }

    }

}