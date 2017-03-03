using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A buyer data object.
    ///
    public class User : IData, IPrincipal
    {
        internal string wx; // weixin openid

        internal string credential;

        internal string name; // user name

        internal string wxname; // weixin nickname

        internal string tel;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(wxname), ref wxname);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(wx), wx);
            o.Put(nameof(wxname), wxname);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
        }

        //
        // interface IPrincipal

        public string Credential => credential;

        public Token ToToken()
        {
            return new Token()
            {
                key = wx,
                wx = wx,
                name = name ?? wxname,
                roles = Token.ROLE_USER
            };
        }
    }
}