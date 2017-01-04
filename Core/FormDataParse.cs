using System;

namespace Greatbone.Core
{
    ///
    /// To parse multipart/form-data content.
    ///
    public struct FormDataParse
    {
        const string ContentDisposition = "Content-Disposition:";

        static readonly ParseException ParseEx = new ParseException("multipart form exception");

        readonly string boundary;

        readonly byte[] bytebuf;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormDataParse(string boundary, byte[] bytebuf, int count)
        {
            this.boundary = boundary;
            this.bytebuf = bytebuf;
            this.count = count;
            this.str = new Str(256);
        }

        int this[int index] => bytebuf[index];

        public Form Parse()
        {
            if (count == 0) return Form.Empty;

            int p = (bytebuf[0] == '?') ? 1 : 0;

            if (p >= count - 1) return Form.Empty;

            Form frm = new Form();
            for (;;)
            {
                if (p >= count) return frm;

                // name value
                string name = ParseName(ref p);
                string value = ParseValue(ref p);
                frm.Add(name, value);
            }
        }

        public void ParseEvents(Action<WebEventContext> e)
        {

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