using System;

namespace Greatbone
{
    /// <summary>
    /// To specify basic user interface-related information for a nodule (work or procedure) object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        readonly byte group;

        public UiAttribute(string label = null, string tip = null, byte group = 0)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.group = group;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A grouping number that refers to a particular functionality.
        /// </summary>
        public byte Group => group;
    }
}