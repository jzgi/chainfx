using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct WeiXin : IData
    {
        internal string appid;
        internal string appsecret;
        internal string addr;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(appid), ref appid);
            i.Get(nameof(appsecret), ref appsecret);
            i.Get(nameof(addr), ref addr);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(appid), appid);
            o.Put(nameof(appsecret), appsecret);
            o.Put(nameof(addr), addr);
        }
    }
}