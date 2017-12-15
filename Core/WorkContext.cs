using System;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>
    /// The creation environment for a particular work instance.
    /// </summary>
    public class WorkContext
    {
        // either the identifying name for a fixed work or the constant var for a variable work
        readonly string name;

        public WorkContext(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public ServiceContext ServiceCtx { get; internal set; }

        public WorkContext Parent { get; internal set; }

        public Work Work { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        // to obtain a string key from a data object.
        public Delegate Keyer { get; internal set; }
    }
}