using System.IO;

namespace Greatbone.Core
{
    public class WebArg
    {
        internal string key;


        public string Key => key;

        public object State { get; internal set; }

        public bool IsMultiplex { get; internal set; }

        public IParent Parent { get; internal set; }

        public virtual string Folder { get; internal set; }

        public WebService Service { get; internal set; }


        public string GetPath(string file)
        {
            return Path.Combine(Folder, file);
        }
    }
}