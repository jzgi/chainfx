using System;

namespace Greatbone.Core
{
    ///
    /// To parse multipart/form-data content, with the part Content-Length extension.
    ///
    public struct FormMpParse
    {
        static readonly Form Empty = new Form(true);

        static readonly ParseException ParseEx = new ParseException("multipart error");

        readonly byte[] bound;

        readonly byte[] buffer;

        readonly int length;

        public FormMpParse(string boundary, byte[] buffer, int length)
        {
            // init byte array
            int len = boundary.Length;
            byte[] a = new byte[4 + len];
            int i = 0;
            a[i++] = (byte)'\r';
            a[i++] = (byte)'\n';
            a[i++] = (byte)'-';
            a[i++] = (byte)'-';
            for (int k = 0; k < len; k++, i++)
            {
                a[i] = (byte)boundary[k];
            }
            this.bound = a;

            this.buffer = buffer;
            this.length = length;
            EventContext = null;
        }

        public WebEventContext EventContext { get; internal set; }

        public Form Parse()
        {
            return Parse(null);
        }

        public Form Parse(Action<WebEventContext> handler)
        {
            // locality
            byte[] bound = this.bound;
            byte[] buffer = this.buffer;
            int length = this.length;

            // UTF-8 header builder
            Header hdr = new Header(128);
            Str str = new Str(128);

            // keep local for speed
            int boundlen = bound.Length;

            // shall init lately
            Form frm = null;
            int p = 0;

            // skip first bound line whatever
            for (;;)
            {
                if (buffer[p++] == '\r' && buffer[p++] == '\n') break;
            }

            // parse parts
            for (;;)
            {
                string name = null;
                string filename = null;
                string ctyp = null;
                string clen = null;

                // parse headers
                for (;;)
                {
                    hdr.Clear();

                    // parse a header line
                    for (;;)
                    {
                        if (p >= this.length - 2) throw ParseEx;
                        byte b;
                        if ((b = buffer[p++]) == '\r' && buffer[p++] == '\n') break;
                        hdr.Accept(b); // lineup the byte
                    }
                    if (hdr.Count == 0) // if empty line then quit header section
                    {
                        break;
                    }
                    if (name == null && hdr.Check("Content-Disposition"))
                    {
                        name = hdr.SeekParam("name");
                        filename = hdr.SeekParam("filename");
                    }
                    else if (ctyp == null && hdr.Check("Content-Type"))
                    {
                        ctyp = hdr.GetVvalue();
                    }
                    else if (clen == null && hdr.Check("Content-Length"))
                    {
                        clen = hdr.GetVvalue();
                    }
                }

                // get part's content
                str.Clear();
                bool plain = ctyp == null || "text/plain".Equals(ctyp);
                int start = p; // mark down content start
                if (EventContext == null) // parse till bound to get content
                {
                    int idx = 0; // index on bound 
                    for (;;)
                    {
                        byte b = buffer[p++];
                        if (b == bound[idx])
                        {
                            idx++;
                            if (idx >= boundlen) // fully matched the bound accumulatively
                            {
                                if (frm == null) frm = new Form(true) { Buffer = buffer };
                                if (plain)
                                    frm.Add(name, str.ToString());
                                else
                                    frm.Add(name, filename, start, p - start - boundlen);
                                // goto the ending CRLF/-- check
                                break;
                            }
                        }
                        else if (idx > 0) // if fail-match
                        {
                            if (plain) // re-add
                            {
                                for (int i = 0; i < idx; i++)
                                {
                                    str.Accept(bound[i]);
                                }
                            }
                            idx = 0; // reset
                        }
                        else
                        {
                            if (plain) str.Accept(b);
                        }
                    }
                }
                else // it is event context
                {
                    int len;
                    if (int.TryParse(clen, out len))
                    {
                        object cont = Contentize(ctyp, buffer, start, len);
                        // handle the event context
                        EventContext.Reset(234, name, cont);
                        handler(EventContext);
                    }
                    // skip bound
                    p += boundlen;
                }

                // check if any more part
                if (buffer[p++] == '\r' && buffer[p++] == '\n')
                {
                    continue;
                }
                break;
            } // parts
            return frm ?? Empty;
        }

        object Contentize(string typ, byte[] buf, int offset, int count)
        {
            return null;
        }
    }
}