namespace Greatbone.Core
{
    ///
    /// The embedded settings related to authetication.
    ///
    public class AuthConfig : IData
    {

        // mask for encoding/decoding token
        public int mask;

        // order for encoding/decoding token
        public int repos;

        public int maxage;

        public string url;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(mask), ref mask);
            i.Get(nameof(repos), ref repos);
            i.Get(nameof(maxage), ref maxage);
            i.Get(nameof(url), ref url);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(mask), mask);
            o.Put(nameof(repos), repos);
            o.Put(nameof(maxage), maxage);
            o.Put(nameof(url), url);
        }
    }
}