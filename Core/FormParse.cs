namespace Greatbone.Core
{
    ///
    /// To parse application/x-www-form-urlencoded octets or a character string.
    ///
    public struct FormParse
    {
        static readonly Form Empty = new Form(false);

        static readonly ParseException ParseEx = new ParseException("form");

        readonly byte[] bytebuf;

        readonly string strbuf;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormParse(byte[] bytebuf, int count)
        {
            this.bytebuf = bytebuf;
            this.strbuf = null;
            this.count = count;
            this.str = new Str(256);
        }

        public FormParse(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.count = strbuf?.Length ?? 0;
            this.str = new Str(256);
        }

        int this[int index] => bytebuf?[index] ?? (int) strbuf[index];

        public Form Parse()
        {
            if (count == 0) return Empty;

            int p = (this[0] == '?') ? 1 : 0;

            if (p >= count - 1) return Empty;

            Form frm = new Form(false);
            for (;;)
            {
                if (p >= count) return frm;

                // name value
                string name = ParseName(ref p);
                string value = ParseValue(ref p);
                frm.Add(name, value);
            }
        }

        string ParseName(ref int pos)
        {
            str.Clear();
            int p = pos;
            for (;;)
            {
                int b = this[p++];
                if (p >= count)
                {
                    return null;
                }
                if (b == '=')
                {
                    pos = p;
                    return str.ToString();
                }
                else
                {
                    str.Accept(b);
                }
            }
        }

        string ParseValue(ref int pos)
        {
            str.Clear();
            int p = pos;
            for (;;)
            {
                if (p >= count)
                {
                    pos = p;
                    return str.ToString();
                }
                int b = this[p++];
                if (b == '&')
                {
                    pos = p;
                    return str.ToString();
                }
                else if (b == '+')
                {
                    str.Accept(' ');
                }
                else if (b == '%') // percent-encoding %xy
                {
                    if (p >= count) throw ParseEx;
                    int x = this[p++];
                    if (p >= count) throw ParseEx;
                    int y = this[p++];

                    str.Accept(Dv(x) << 4 | Dv(y));
                }
                else
                {
                    str.Accept(b);
                }
            }
        }

        // return digit value
        static int Dv(int h)
        {
            int v = h - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }
            else
            {
                v = h - 'A';
                if (v >= 0 && v <= 5) return 10 + v;
            }
            return 0;
        }
    }
}