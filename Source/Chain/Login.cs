namespace SkyCloud.Chain
{
    public class Login : IData
    {
        public static readonly Login Empty = new Login();

        public const byte ID = 1, PRIVACY = 2;

        // types
        public static readonly Map<short, string> Typs = new Map<short, string>
        {
            {0, "APP"},
            {1, "admin"},
        };

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "Disabled"},
            {1, "Enabled"},
        };

        internal string id;

        internal short typ;

        internal string name;

        internal string credential;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(name), ref name);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Get(nameof(credential), ref credential);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(typ), typ);
            s.Put(nameof(name), name);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(status), status);
        }
    }
}