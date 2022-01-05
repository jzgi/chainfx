using System;

namespace SkyChain.Db
{
    /// <summary>
    /// Mark version of chain operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public abstract class VerAttribute : Attribute
    {
        readonly short major;

        readonly short minor;

        protected VerAttribute(short major, short minor)
        {
            this.major = major;
            this.minor = minor;
        }

        public short Major => major;

        public short Minor => minor;
    }
}