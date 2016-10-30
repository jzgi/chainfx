namespace Greatbone.Core
{

    ///
    /// <summary>
    /// To parse application/x-www-form-urlencoded octets.
    /// </summary>
    ///
    public struct FormParse
    {
        static readonly ParseException FormatEx = new ParseException("wrong form Format");

        readonly byte[] buffer;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormParse(byte[] buffer, int count)
        {
            this.buffer = buffer;
            this.count = count;
            this.str = new Str(256);
        }

        public Form Parse()
        {
            Form frm = new Form();
            int p = -1;
            for (;;)
            {
                if (p >= count)
                {
                    return frm;
                }
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
                byte b = buffer[++p];
                if (p >= count) throw FormatEx;
                if (b == '=')
                {
                    pos = p;
                    return str.ToString();
                }
                else
                {
                    str.Add(b);
                }
            }
        }

        string ParseValue(ref int pos)
        {
            str.Clear();
            int p = pos;
            for (;;)
            {
                byte b = buffer[++p];
                if (p >= count || b == '&')
                {
                    pos = p;
                    return str.ToString();
                }
                else if (b == '+')
                {
                    str.Add((byte)' ');
                }
                else if (b == '%') // percent-encoding %xy
                {
                    char x = (char)buffer[++p];
                    if (p >= count) throw FormatEx;
                    char y = (char)buffer[++p];
                    if (p >= count) throw FormatEx;

                    str.Add((byte)(Dv(x) << 4 | Dv(y)));
                }
                else
                {
                    str.Add(b);
                }
            }
        }

        // return digit value
        static int Dv(char h)
        {
            int v = h - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }
            else
            {
                v = h - 'a';
                if (v >= 0 && v <= 5) return 10 + v;
            }
            return 0;
        }

    }
}