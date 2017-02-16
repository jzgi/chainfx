using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct Login : IData
    {
        internal string id;
        internal string password;
        internal bool remember;
        internal string orig;

        internal string err;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(password), ref password);
            i.Get(nameof(remember), ref remember);
            i.Get(nameof(orig), ref orig);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id, Placeholder: "用户手机号/商户编号", Max: 11, Required: true);
            o.Put(nameof(password), password);
            o.Put(nameof(remember), remember);
            o.Put(nameof(orig), orig, Label: string.Empty);
        }

        public string CalcCredential() => TextUtility.MD5(id + ':' + password);

        public bool IsShop => id.Length == 6 && char.IsDigit(id[0]);

        public bool IsUser => id.Length == 11 && char.IsDigit(id[0]);
    }
}