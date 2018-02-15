namespace Greatbone.Core
{
    /// <summary>
    /// To parse application/x-www-form-urlencoded octets or a character string.
    /// </summary>
    public struct FormParser : IParser<Form>
    {
        static readonly Form Empty = new Form(false);

        static readonly ParserException ParserEx = new ParserException("error parsing form");

        readonly byte[] bytebuf;

        readonly string strbuf;

        readonly int length;

        // UTF-8 string builder
        readonly Str str;

        public FormParser(byte[] bytebuf, int length)
        {
            this.bytebuf = bytebuf;
            this.strbuf = null;
            this.length = length;
            this.str = new Str(256);
        }

        public FormParser(string strbuf)
        {
            this.bytebuf = null;
            this.strbuf = strbuf;
            this.length = strbuf?.Length ?? 0;
            this.str = new Str(256);
        }

        int this[int index] => bytebuf?[index] ?? (int) strbuf[index];

        public Form Parse()
        {
            if (length == 0) return Empty;

            int p = (this[0] == '?') ? 1 : 0;

            if (p >= length - 1) return Empty;

            Form frm = new Form(false);
            for (;;)
            {
                if (p >= length) return frm;

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
                if (p >= length)
                {
                    return null;
                }
                int b = this[p++];
                if (b == '=')
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
                    if (p >= length) throw ParserEx;
                    int x = this[p++];
                    if (p >= length) throw ParserEx;
                    int y = this[p++];

                    str.Accept(Dv(x) << 4 | Dv(y));
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
                if (p >= length)
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
                    if (p >= length) throw ParserEx;
                    int x = this[p++];
                    if (p >= length) throw ParserEx;
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