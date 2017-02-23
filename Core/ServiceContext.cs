using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Greatbone.Core
{
    ///
    /// The configurative settings for a web service.
    /// 
    /// <remark>
    /// The strong semantic allows the web folder hierarchy to be established during object initialization.
    /// </remark>
    /// <code>
    /// public class FooService : Service
    /// {
    ///     public FooService(ServiceContext sc) : base(sc)
    ///     {
    ///         Create&lt;BarFolder&gt;("bar");
    ///     }
    /// }
    /// </code>
    ///
    public class ServiceContext : FolderContext, IData
    {
        /// The shard identifier when one service is divided into many shards
        public string shard;

        /// The bind addresses in URI (scheme://host:port) format.
        public string addresses;

        /// The authentication attributes.
        public AuthConfig auth;

        /// The database connectivity attributes.
        public DbConfig db;

        /// The logging level, default to warning (3)
        public int logging = 3;

        public Dictionary<string, string> cluster;

        // connection string
        volatile string connstr;

        public ServiceContext(string name)
        {
            this.name = name;
        }

        ///
        /// Let the file directory name same as the service name.
        ///
        public override string Directory => name;

        ///
        /// The json object model.
        ///
        public JObj Json { get; private set; }

        public bool? LoadedOk { get; private set; }

        public JObj Extra => Json?["extra"];

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(shard), ref shard);
            i.Get(nameof(addresses), ref addresses);
            i.Get(nameof(db), ref db);
            i.Get(nameof(logging), ref logging);
            i.Get(nameof(cluster), ref cluster);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(shard), shard);
            o.Put(nameof(addresses), addresses);
            o.Put(nameof(db), db);
            o.Put(nameof(logging), logging);
            o.Put(nameof(cluster), cluster);
        }

        ///
        /// Try to load from the $web.json file.
        ///
        public bool TryLoad()
        {
            string path = GetFilePath("$web.json");
            if (File.Exists(path))
            {
                JObj jo = JsonUtility.FileToJObj(path);
                if (jo != null)
                {
                    Json = jo;
                    ReadData(jo); // override
                    return (LoadedOk = true).Value;
                }
            }
            return (LoadedOk = false).Value;
        }

        public string ConnectionString
        {
            get
            {
                if (connstr == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Host=").Append(db.host);
                    sb.Append(";Port=").Append(db.port);
                    sb.Append(";Database=").Append(db.database ?? Name);
                    sb.Append(";Username=").Append(db.username);
                    sb.Append(";Password=").Append(db.password);
                    sb.Append(";Read Buffer Size=").Append(1024 * 32);
                    sb.Append(";Write Buffer Size=").Append(1024 * 32);
                    sb.Append(";No Reset On Close=").Append(true);

                    connstr = sb.ToString();
                }
                return connstr;
            }
        }
        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public string Encrypt(IData token)
        {
            if (auth == null) return null;

            JsonContent cont = new JsonContent(true, true, 4096); // borrow
            cont.Put(null, token);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int mask = auth.mask;
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            char[] charbuf = new char[count * 2]; // the target 
            int p = 0;
            for (int i = 0; i < count; i++)
            {
                // masking
                int b = bytebuf[i] ^ masks[i % 4];

                //transform
                charbuf[p++] = HEX[(b >> 4) & 0x0f];
                charbuf[p++] = HEX[(b) & 0x0f];

                // reordering
            }
            // return pool
            BufferUtility.Return(bytebuf);

            return new string(charbuf, 0, charbuf.Length);
        }

        public string Decrypt(string tokenstr)
        {
            if (auth == null) return null;

            int mask = auth.mask;
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            int len = tokenstr.Length / 2;
            Text str = new Text(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte)(Dv(tokenstr[p++]) << 4 | Dv(tokenstr[p++]));

                // masking
                str.Accept((byte)(b ^ masks[i % 4]));
            }
            return str.ToString();
        }


        // return digit value
        static int Dv(char hex)
        {
            int v = hex - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }
            v = hex - 'A';
            if (v >= 0 && v <= 5) return 10 + v;
            return 0;
        }

        public void SetBearerCookie(ActionContext ac, IData token)
        {
            StringBuilder sb = new StringBuilder("Bearer=");
            string tokenstr = Encrypt(token);
            sb.Append(tokenstr);
            sb.Append("; HttpOnly");
            if (auth.maxage != 0)
            {
                sb.Append("; Max-Age=").Append(auth.maxage);
            }
            // detect domain from the Host header
            string host = ac.Header("Host");
            if (!string.IsNullOrEmpty(host))
            {
                // if the last part is not numeric
                int lastdot = host.LastIndexOf('.');
                if (lastdot > -1 && !char.IsDigit(host[lastdot + 1])) // a domain name is given
                {
                    int dot = host.LastIndexOf('.', lastdot - 1);
                    if (dot != -1)
                    {
                        string domain = host.Substring(dot + 1);
                        sb.Append("; Domain=").Append(domain);
                    }
                }
            }
            // set header
            ac.SetHeader("Set-Cookie", sb.ToString());
        }
    }
}