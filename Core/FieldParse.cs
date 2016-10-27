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
        readonly string value;

        // parsing position
        int pos;

        readonly Str str;

        internal FieldParse(string v)
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

        public bool Match(String str)
        {
            int p;
            if (value.IndexOf(str, pos) != -1)
            {
                pos += str.Length; // move forward
                return true;
            }
            return false;
        }

        public String Parameter(String nameEq)
        {
            str.Clear();

            // parameter position
            int param = value.IndexOf(nameEq, pos);
            if (param == -1)
            {
                param = value.LastIndexOf(nameEq, pos);
            }
            if (param != -1)
            {
                int start = param + nameEq.Length;
                bool quot = value[start] == '"';
                int p = start;
                for (;;)
                {
                    char c = value[p++];
                    if (c == '/')
                    {
                        str.Add(value[p++]); // add the following char
                    }
                    else if (c == '"')
                    {
                        quot = true;
                    }
                    else
                    {
                        str.Add(c);
                    }
                }
            }
            return null;
        }

    }
}