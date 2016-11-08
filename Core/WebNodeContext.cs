using System.IO;

namespace Greatbone.Core
{

    ///
    /// The context for a node in the controlling hierarchy.
    ///
    public class WebNodeContext
    {
        internal string key;


        public string Key => key;

        public object State { get; internal set; }

        public bool HasVar { get; internal set; }

        public IParent Parent { get; internal set; }

        public virtual string Folder { get; internal set; }

        public WebService Service { get; internal set; }


        public string GetFilePath(string file)
        {
            return Path.Combine(Folder, file);
        }
    }
}