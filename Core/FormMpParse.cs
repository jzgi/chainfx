using System;

namespace Greatbone.Core
{
    ///
    /// To parse multipart/form-data content, with the part Content-Length extension.
    ///
    public struct FormMpParse
    {
        static readonly Form Empty = new Form(true);

        const string ContentDisposition = "Content-Disposition:";

        static readonly ParseException ParseEx = new ParseException("multipart parse error");

        readonly string bound;

        readonly byte[] bytebuf;

        readonly int count;

        // UTF-8 string builder
        readonly Str str;

        public FormMpParse(string boundary, byte[] bytebuf, int count)
        {
            this.bound = "--" + boundary;
            this.bytebuf = bytebuf;
            this.count = count;
            this.str = new Str(256);
        }

        public Form Parse()
        {
            if (count == 0) return Empty;

            Form frm = new Form(true);
            int p = 0;
            for (;;)
            {
                // bound
                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    byte b = bytebuf[++p];
                    if (b == '\r' && bytebuf[++p] == '\n') break;
                    str.Accept(b);
                }

                str.Match(bound);

                str.Clear(); // parse name

                for (;;)
                {
                    if (p >= count - 1) throw ParseEx;
                    int b = bytebuf[++p];
                    if (b == '"') break; // meet second quote
                    str.Add((char) b);
                }

                string name = ParseName(ref p);
                string value = null;
                frm.Add(name, value);

                frm.Add(name, "", null, 0);
            }
        }

        public void ParseEvents(Action<WebActionContext> a)
        {

        }
        string ParseName(ref int pos)
        {
            str.Clear();
            int p = pos;
            for (;;)
            {
                int b = bytebuf[p++];
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
    }
}