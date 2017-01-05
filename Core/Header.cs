namespace Greatbone.Core
{
    ///
    /// To parse HTTP header.
    ///
    public class Header : Str
    {
        static readonly ParseException ParseEx = new ParseException("error in header parsing");

        // start index of the value field
        int vstart;

        // current settled position
        int mark;

        public Header(int capacity = 256) : base(capacity)
        {
        }

        public override void Clear()
        {
            vstart = 0;
            mark = 0;
        }

        public bool NameIs(string hdrname)
        {
            int len = hdrname.Length;

            if (count < len + 2) return false;

            for (int i = 0; i < len; i++)
            {
                if (charbuf[i] != hdrname[i])
                {
                    return false;
                }
            }
            if (charbuf[len] != ':') return false;

            vstart = mark = len + 1;
            return true;
        }

        public string GetVvalue() => new string(charbuf, vstart, 0);

        static bool IsNamePart(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
        }

        public string SeekParam(string param)
        {
            int len = param.Length;

            int pos = mark;
            for (;;)
            {
                // seek for an eq
                for (;;)
                {
                    if (++pos >= count) return null;
                    if (charbuf[pos] == '=') break;
                }

                int f = pos - len; // first char position
                if (IsNamePart(charbuf[f - 1])) continue;

                bool found = true;
                for (int i = 0; i < len; i++)
                {
                    if (charbuf[f + i] != param[i])
                    {
                        found = false;
                        break;
                    }
                }
                if (!found) continue;

                // get value after eq

                int start = pos + 1;
                bool quot = charbuf[start] == '"';
                if (quot)
                {
                    int p = start;
                    for (;;)
                    {
                        if (charbuf[p] == '"')
                        {
                            mark = p;
                            return new string(charbuf, start, p - start);
                        }
                    }
                }
            }
        }
    }
}