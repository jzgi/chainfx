namespace SkyCloud.Chain
{
    public class Login : IData
    {
        public static readonly Login Empty = new Login();

        public const byte ID = 1, PRIVACY = 2, LATER = 4, EXTRA = 0x10;

        // status
        public static readonly Map<short, string> Roles = new Map<short, string>
        {
            {0, "APP"},
            {1, "Admin"},
        };

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "Disabled"},
            {1, "Enabled"},
        };

        internal string id;

        internal string name;

        internal string credential;

        internal short role;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(credential), ref credential);
            s.Get(nameof(role), ref role);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(credential), credential);
            s.Put(nameof(role), role);
            s.Put(nameof(status), status);
        }
    }
}