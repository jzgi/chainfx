using System.IO;

namespace Greatbone.Core
{
    ///
    /// The context for a particular node in the web folder hierarchy.
    ///
    public class WebFolderContext
    {
        internal string key;

        public string Key => key;

        public object State { get; internal set; }

        public bool Var { get; internal set; }

        public int Level { get; internal set; }

        public WebFolder Parent { get; internal set; }

        public virtual string Directory { get; internal set; }

        public WebService Service { get; internal set; }


        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }
    }
}