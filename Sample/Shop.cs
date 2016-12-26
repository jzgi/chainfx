using System;
using Greatbone.Core;
using static Greatbone.Core.BitsUtility;

namespace Greatbone.Sample
{
    ///
    /// A shop data object.
    ///
    public class Shop : IToken, IData
    {
        public static readonly Shop Empty = new Shop();

        internal string id;
        internal string name;
        internal string credential;
        internal string tel;
        internal string district;
        internal string appid;
        internal string appsecret;
        internal bool wepay; // wepay enabled
        internal bool disabled;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(name), ref name);
            if (bits.Has(RESV)) src.Get(nameof(credential), ref credential);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(district), ref district);
            src.Get(nameof(appid), ref appid);
            src.Get(nameof(appsecret), ref appsecret);
            src.Get(nameof(wepay), ref wepay);
            src.Get(nameof(disabled), ref disabled);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(name), name);
            if (bits.Has(RESV)) snk.Put(nameof(credential), credential);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(district), district);
            snk.Put(nameof(appid), appid);
            snk.Put(nameof(appsecret), appsecret);
            snk.Put(nameof(wepay), wepay);
            snk.Put(nameof(disabled), disabled);
        }

        //
        // ACCESS_TOKEN
        //

        int expiry;
        int last;
        string accessToken; // cached access token

        public string AccessToken
        {
            get
            {
                lock (this)
                {
                    int ticks = Environment.TickCount;
                    if ((ticks - last) / 1000 > expiry) // if expired
                    {
                        // update access token

                        accessToken = WeChatUtility.GetAccessToken(appid, appsecret);
                    }
                    return accessToken;
                }
            }
        }

        public void CreateMenu()
        {

        }

    }
}