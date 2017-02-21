namespace Greatbone.Core
{
    ///
    /// The web authetication configuration embedded in a service context.
    ///
    public class AuthConfig : IData
    {
        // mask for encoding/decoding token
        public int mask;

        // repositioning factor for encoding/decoding token
        public int pose;

        // The number of seconds that a signon durates, or null if session-wide.
        public int maxage;

        // The service instance that does signon. Can be null if local
        public string moniker;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(mask), ref mask);
            i.Get(nameof(pose), ref pose);
            i.Get(nameof(maxage), ref maxage);
            i.Get(nameof(moniker), ref moniker);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(mask), mask);
            o.Put(nameof(pose), pose);
            o.Put(nameof(maxage), maxage);
            o.Put(nameof(moniker), moniker);
        }
    }
}