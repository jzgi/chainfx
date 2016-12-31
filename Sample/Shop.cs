using System;
using Greatbone.Core;
using NpgsqlTypes;
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
        internal NpgsqlPoint loc;
        internal string prov;
        internal string city;
        internal short status;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(name), ref name);
            if (bits.Has(HIDDEN)) src.Get(nameof(credential), ref credential);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(loc), ref loc);
            src.Get(nameof(prov), ref prov);
            src.Get(nameof(city), ref city);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(name), name);
            if (bits.Has(HIDDEN)) snk.Put(nameof(credential), credential);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(loc), loc);
            snk.Put(nameof(prov), prov);
            snk.Put(nameof(city), city);
            snk.Put(nameof(status), status);
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

                        // accessToken = WeChatUtility.GetAccessToken(city, appsecret);
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