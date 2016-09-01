using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>A user record that is a web access token for all the services. </summary>
    public class User : IToken, ISerial
    {
        string id;

        string name;

        string email;

        string[] roles;

        public string Login => id;

        public string[] Roles => roles;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(name), ref name);
            r.Read(nameof(email), ref email);
            r.Read(nameof(roles), ref roles);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(name), name);
            w.Write(nameof(email), email);
            w.Write(nameof(roles), roles);
        }

        public static string Encrypt(string orig)
        {
            return null;
        }

        public static string Decrypt(string src)
        {
            return null;
        }

    }
}