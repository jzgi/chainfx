using System;

namespace Greatbone.Core
{
    ///
    /// To parse multipart/form-data content, with the part Content-Length extension.
    ///
    public struct FormMpParse
    {
        static readonly Form Empty = new Form(true);

        static readonly ParseException ParseEx = new ParseException("multipart parse error");

        readonly string bound;

        readonly byte[] bytebuf;

        readonly int count;

        // UTF-8 header builder
        readonly Header hdr;

        public FormMpParse(string boundary, byte[] bytebuf, int count)
        {
            this.bound = "--" + boundary;
            this.bytebuf = bytebuf;
            this.count = count;
            this.hdr = new Header(256);
            Context = null;
        }

        public Form Parse()
        {
            return Parse(null);
        }

        public WebEventContext Context { get; set; }

        public Form Parse(Action<WebEventContext> a)
        {
            byte[] buf = this.bytebuf;

            if (count == 0) return Empty;

            Form frm = new Form(true);
            int p = 0;

            // first bound

            // parts
            for (;;)
            {
                string name = null;
                string filename;
                string typ = null;
                string length = null;

                int partstart;
                string value = null;

                // parse headers
                for (;;)
                {
                    hdr.Clear();

                    // parse a header
                    for (;;)
                    {
                        if (p >= count - 1) throw ParseEx;

                        int b = buf[++p];
                        if (b == '\r' && buf[++p] == '\n') break;
                        hdr.Accept(b);
                    }

                    if (name == null && hdr.NameIs("Content-Disposition"))
                    {
                        // name, filename, time
                        name = "";
                    }
                    else if (typ == null && hdr.NameIs("Content-Type"))
                    {
                        typ = "";
                    }
                    else if (length == null && hdr.NameIs("Content-Length"))
                    {
                        length = "";
                    }
                    else if (hdr.Count == 0) // if empty line
                    {
                        break;
                    }
                }

                // body and bound
                if (length == null) // standard behavior
                {
                    frm.Add(name, value);
                    frm.Add(name, "", null, 0);
                }
                else // directly
                {
                    int len;
                    if (int.TryParse(length, out len))
                    {
                        a(Context);
                    }
                    // bound
                }
            }
        }
    }
}