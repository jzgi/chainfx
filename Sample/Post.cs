using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>A brand object.</summary>
    /// <example>
    ///     Brand o  new Brand(){}
    /// </example>
    public class Post : ISerial
    {
        internal int id;

        internal DateTime publishat;

        internal bool comment;

        internal string authorid;

        internal List<Comment> comments;

        internal string text;

        public long ModifiedOn { get; set; }

        ///
        /// <summary>Returns the key of the brand object.</summary>
        public string Key { get; }

        public void ReadFrom(ISerialReader r)
        {
            throw new System.NotImplementedException();
        }

        public void WriteTo(ISerialWriter w)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Comment : ISerial
    {
        public void ReadFrom(ISerialReader r)
        {
        }

        public void WriteTo(ISerialWriter w)
        {
            throw new NotImplementedException();
        }
    }
}