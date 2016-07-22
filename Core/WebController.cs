using System;
using System.IO;

namespace Greatbone.Core
{
    ///
    /// Represents an abstract controller.
    ///
    public abstract class WebController : IMember
    {
        public string StaticPath { get; }

        // the corresponding static file folder, can be null
        public Set<Static> Statics { get; }

        // the default static file, can be null
        public Static DefaultStatic { get; }

        ///
        /// The parent service that this sub-controller is added to
        ///
        public IParent Parent { get; internal set; }

        ///
        /// The service that this component resides in.
        ///
        public WebService Service { get; }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key { get; }


        internal WebController(WebCreationContext wcc)
        {
            Key = wcc.Key;
            Parent = wcc.Parent;
            Service = wcc.Service;
            StaticPath = wcc.StaticPath;

            // load static files, if any
            if (StaticPath != null)
            {
                Statics = new Set<Static>(256);
                foreach (string path in Directory.GetFiles(StaticPath))
                {
                    string file = Path.GetFileName(path);
                    string ext = Path.GetExtension(path);
                    string ctype;
                    if (Static.TryGetType(ext, out ctype))
                    {
                        byte[] content = File.ReadAllBytes(path);
                        DateTime modified = File.GetLastWriteTime(path);
                        Static sta = new Static
                        {
                            Key = file.ToLower(),
                            ContentType = ctype,
                            Content = content,
                            Modified = modified
                        };
                        Statics.Add(sta);
                        if (file.StartsWith("default."))
                        {
                            DefaultStatic = sta;
                        }
                    }
                }
            }
        }
    }
}