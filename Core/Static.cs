using System;

namespace Greatbone.Core
{
    ///
    /// Represents a static file.
    ///
    public class Static : IMember
    {
        public string Name { get; internal set; }

        public string Path { get; internal set; }

        public string ContentType { get; internal set; }

        public byte[] Content { get; internal set; }

        public DateTime Modified { get; internal set; }

        public int Length => Content.Length;

        public string Key => Path;
    }
}