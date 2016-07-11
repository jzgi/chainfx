using System.Security.Cryptography.X509Certificates;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// An internet celebrity
    public class User : IDump
    {
        string id;

        string name;

        string mobile;


        public void From(IInput i)
        {
            i.GotStart();
            i.Got(nameof(id), out id);
            i.Got(nameof(name), out name);
            i.Got(nameof(mobile), out mobile);
            i.GotEnd();
        }

        public void To(IOutput o)
        {
            o.PutStart();
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(mobile), mobile);
            o.PutEnd();
        }
    }
}