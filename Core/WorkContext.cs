using System;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>
    /// The creation environment for a particular work instance.
    /// </summary>
    public class WorkContext
    {
        readonly string name;

        public WorkContext(string name)
        {
            this.name = name;
        }

        public Service Service { get; internal set; }

        public string Name => name;

        // to obtain a string key from a data object.
        public Delegate Keyer { get; internal set; }

        public object Attach { get; internal set; }

        public int Level { get; internal set; }

        public Work Parent { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }
    }
}