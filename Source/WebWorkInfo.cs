using System;
using System.IO;

namespace Greatbone.Service
{
    /// <summary>
    /// The creation environment for a particular work instance.
    /// </summary>
    public sealed class WebWorkInfo
    {
        // either an identifying name for a fixed work or the constant _var_ for a variable work
        readonly string name;

        public WebWorkInfo(string name)
        {
            this.name = name;
        }

        internal UiAttribute Ui { get; set; }

        internal AuthorizeAttribute Authorize { get; set; }

        public string Name => name;

        public WebServer Service { get; internal set; }

        public WebWork Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string Pathing { get; internal set; }

        // to resolve from the principal object.
        public Func<IData, object> Accessor { get; internal set; }

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }
    }
}