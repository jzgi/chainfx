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

        readonly byte[] bound;

        readonly byte[] buf;

        readonly int count;

        public FormMpParse(string boundary, byte[] buf, int count)
        {
            // init byte array
            int len = boundary.Length;
            byte[] a = new byte[4 + len];
            int i = 0;
            a[i++] = (byte)'\r'; a[i++] = (byte)'\n'; a[i++] = (byte)'-'; a[i++] = (byte)'-';
            for (int k = 0; k < len; k++, i++)
            {
                a[i] = (byte)boundary[k];
            }
            this.bound = a;

            this.buf = buf;
            this.count = count;
            Event = null;
        }

        public WebEventContext Event { get; internal set; }

        public Form Parse()
        {
            return Parse(null);
        }

        public Form Parse(Action<WebEventContext> handler)
        {
            // UTF-8 header builder
            Header hdr = new Header(256);

            // keep local for speed
            int boundlen = bound.Length;

            // shall init lately
            Form frm = null;
            int p = 0;

            // skip first bound line whatever
            for (;;) { if (buf[p++] == '\r' && buf[p++] == '\n') break; }

            // parse parts
            for (;;)
            {
                string name = null;
                string filename = null;
                string time;
                string typ = null;
                string length = null;

                // parse headers
                for (;;)
                {
                    hdr.Clear();

                    // parse a header line
                    for (;;)
                    {
                        if (p >= count - 2) throw ParseEx;
                        byte b;
                        if ((b = buf[p++]) == '\r' && buf[p++] == '\n') break;
                        hdr.Accept(b); // lineup the byte
                    }

                    if (name == null && hdr.NameIs("Content-Disposition"))
                    {
                        name = hdr.SeekParam("name");
                        filename = hdr.SeekParam("filename");
                        time = hdr.SeekParam("time");
                    }
                    else if (typ == null && hdr.NameIs("Content-Type"))
                    {
                        typ = hdr.GetVvalue();
                    }
                    else if (length == null && hdr.NameIs("Content-Length"))
                    {
                        length = hdr.GetVvalue();
                    }
                    else if (hdr.Count == 0) // if empty line
                    {
                        break;
                    }
                }

                int start = p; // mark down content start
                // get content of the part
                if (length == null) // no Content-Length, parse till bound to get content
                {
                    int idx = 0; // index on bound 
                    for (;;)
                    {
                        byte b = buf[p++];
                        if (b == bound[idx])
                        {
                            idx++;
                            if (idx >= boundlen) // fully matched the bound accumulatively
                            {
                                if (frm == null) frm = new Form(true);
                                frm.Add(name, filename, buf, start);
                            }
                        }
                        else
                        {
                            idx = 0; // reset
                        }
                    }
                }
                else if (Event != null && handler != null) // it is event context
                {
                    int len;
                    if (int.TryParse(length, out len))
                    {
                        object cont = Contentize(typ, buf, start, p - 1);
                        // handle the event context
                        Event.Reset(234, name, "", DateTime.Now, cont);
                        handler(Event);
                    }
                    // skip bound
                    p += boundlen;
                }

                // check if any more part
                if (buf[p++] == '\r' && buf[p++] == '\n')
                {
                    continue;
                }
                break;
            } // parts
            return frm;
        }

        object Contentize(string typ, byte[] buf, int offset, int count)
        {
            return null;
        }
    }
}