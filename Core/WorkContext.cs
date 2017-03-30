using System.IO;

namespace Greatbone.Core
{
    ///
    /// The environment for a particular work.
    ///
    public class WorkContext
    {
        readonly string name;

        public WorkContext(string name)
        {
            this.name = name;
        }

        public Service Service { get; internal set; }

        public string Name => name;

        public AuthorizeAttribute Authorize { get; internal set; }

        public UiAttribute Ui { get; internal set; }

        public int Level { get; internal set; }

        public Work Parent { get; internal set; }

        public string Directory { get; internal set; }

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }
    }
}