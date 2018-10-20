using System;

namespace Greatbone
{
    /// <summary>
    /// The creation environment for a particular work instance.
    /// </summary>
    public class WorkConfig
    {
        // either the identifying name for a fixed work or the constant var for a variable work
        readonly string name;

        public WorkConfig(string name)
        {
            this.name = name;
        }

        internal UiAttribute Ui { get; set; }

        internal AuthorizeAttribute Authorize { get; set; }

        public string Name => name;

        public Service Service { get; internal set; }

        public Work Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string Path { get; internal set; }

        // to resolve from the principal object.
        public Func<IData, object> Principalet { get; internal set; }

        public string GetFilePath(string file)
        {
            return System.IO.Path.Combine(Directory, file);
        }
    }
}