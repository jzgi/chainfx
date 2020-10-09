namespace Skyiah
{
    /// <summary>
    /// To parse multipart/form-data content, with the part Content-Length extension.
    /// </summary>
    public struct FormMpParser : IParser<Form>
    {
        static readonly Form Empty = new Form(true);

        static readonly ParserException ParserEx = new ParserException("parsing multipart error");

        readonly byte[] buffer;

        readonly int length;

        readonly byte[] bound;

        public FormMpParser(byte[] buffer, int length, string boundary)
        {
            // init byte array
            int len = boundary.Length;
            byte[] a = new byte[4 + len];
            int i = 0;
            a[i++] = (byte) '\r';
            a[i++] = (byte) '\n';
            a[i++] = (byte) '-';
            a[i++] = (byte) '-';
            for (int k = 0; k < len; k++, i++)
            {
                a[i] = (byte) boundary[k];
            }
            this.bound = a;
            this.buffer = buffer;
            this.length = length;
        }

        public Form Parse()
        {
            // locality for performance
            byte[] bound_ = this.bound;
            byte[] buffer_ = this.buffer;
            int length_ = this.length;

            // UTF-8 header builder
            Header hdr = new Header(128);
            Text text = new Text(128);

            // keep local for speed
            int boundlen = bound_.Length;

            // shall init lately
            Form frm = null;
            int p = 0;

            // skip first bound line whatever
            for (;;)
            {
                if (buffer_[p++] == '\r' && buffer_[p++] == '\n') break;
            }

            // parse parts
            for (;;)
            {
                string name = null;
                string filename = null;
                string ctype = null;

                // parse headers
                for (;;)
                {
                    hdr.Clear();

                    // parse a header line
                    for (;;)
                    {
                        if (p >= length_ - 2) throw ParserEx;
                        byte b;
                        if ((b = buffer_[p++]) == '\r' && buffer_[p++] == '\n') break;
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
                    else if (ctype == null && hdr.Check("Content-Type"))
                    {
                        ctype = hdr.GetVvalue();
                    }
                }

                // get part's content
                text.Clear();
                bool plain = ctype == null || "text/plain".Equals(ctype);
                int start = p; // mark down content start
                int idx = 0; // index on bound
                for (;;)
                {
                    byte b = buffer_[p++];
                    if (b == bound_[idx])
                    {
                        idx++;
                        if (idx >= boundlen) // fully matched the bound accumulatively
                        {
                            if (frm == null) frm = new Form(true) {Buffer = buffer_};
                            if (plain)
                                frm.Add(name, text.ToString());
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
                                text.Accept(bound_[i]);
                            }
                        }
                        idx = 0; // reset
                    }
                    else
                    {
                        if (plain) text.Accept(b);
                    }
                }

                // check if any more part
                if (buffer_[p++] == '\r' && buffer_[p++] == '\n')
                {
                    continue;
                }
                break;
            } // parts
            return frm ?? Empty;
        }
    }
}