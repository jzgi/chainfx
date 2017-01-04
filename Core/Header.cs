namespace Greatbone.Core
{
    ///
    /// To parse HTTP header.
    ///
    public class Header : Str
    {
        static readonly ParseException ParseEx = new ParseException("error in header parsing");

        // position
        int pos;

        // builder
        readonly Str str;

        public Header(int capacity = 256) : base(capacity)
        {
            pos = 0;
            str = new Str(64);
        }

        public override void Clear()
        {
            pos = 0;
            str.Clear();
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
            pos = len + 1;
            return true;
        }

        public string SeekParam(string param)
        {
            str.Clear();

            int p = pos;

            for (;;)
            {
                if (p >= count) return null;
                char c = charbuf[p++];
                if (c == '=')
                {
                    break;
                }
            }

            int start = pos;

            bool quot = charbuf[pos] == '"';
            if (quot)
            {
                p = pos + 1;
                for (;;)
                {
                    if (p >= charbuf.Length) throw ParseEx;
                    char c = charbuf[p++];
                    if (c == '\\') // quoted-pair
                    {
                        p++;
                        str.Add(charbuf[p]); // add the following char
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
                p = start;
                for (;;)
                {
                    if (p >= charbuf.Length)
                    {
                        pos = p;
                        return str.ToString();
                    }
                    char c = charbuf[p++];
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