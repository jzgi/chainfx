using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Token : IToken, IPersist
    {
        string id;

        public void From(ISource i)
        {
        }

        public void To(ITarget o)
        {
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