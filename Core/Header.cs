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

        public bool CheckName(string hdrname)
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

        static bool IsNameChar(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
        }
        public string SeekParam(string param)
        {
            int len = param.Length;
            int p = mark;

            for (;;)
            {
                if (p >= count) return null;
                char c = charbuf[p];
                if (c == '=')
                {
                    int f = p - len; // first char position
                    if (f < vstart || charbuf[f] != param[0] || (f > vstart && IsNameChar(charbuf[f - 1]))) continue;

                    // match rest chars
                    for (int i = 1; i < len; i++) {
                        if (charbuf[f+i] != param[i]) {

                        }
                    }

                    break;
                }
            }

            int start = mark;

            bool quot = charbuf[p] == '"';
            if (quot)
            {
                p = mark + 1;
                for (;;)
                {
                    if (p >= charbuf.Length) throw ParseEx;
                    char c = charbuf[p++];
                    if (c == '\\') // quoted-pair
                    {
                        p++;
                    }
                    else if (c == '"')
                    {
                        mark = p;
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                p = start;
                for (;;)
                {
                    if (p >= charbuf.Length)
                    {
                        mark = p;
                    }
                    char c = charbuf[p++];
                    if (c == ',' || c == '/' || c == ':' || c == ';') // a delimiter
                    {
                        mark = p;
                    }
                    else
                    {
                    }
                }
            }
        }
    }
}