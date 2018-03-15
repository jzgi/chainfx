using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To specify basic user interface-related information for a nodule (work or procedure) object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        readonly byte flag;

        public UiAttribute(string label = null, string tip = null, byte flag = 0)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.flag = flag;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A grouping number that refers to a particular functionality.
        /// </summary>
        public byte Flag => flag;
    }
}