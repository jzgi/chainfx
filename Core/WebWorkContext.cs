using System.IO;

namespace Greatbone.Core
{

    ///
    /// The context for a particular node in the web work hierarchy.
    ///
    public class WebWorkContext
    {
        internal string key;


        public string Key => key;

        public object State { get; internal set; }

        public bool IsVar { get; internal set; }

        public WebWork Parent { get; internal set; }

        public virtual string Folder { get; internal set; }

        public WebServiceWork Service { get; internal set; }


        public string GetFilePath(string file)
        {
            return Path.Combine(Folder, file);
        }
    }
}