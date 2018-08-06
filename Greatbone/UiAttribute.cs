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

        readonly byte sort;

        public UiAttribute(string label = null, string tip = null, byte sort = 0)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.sort = sort;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A sorting number that refers to a particular functionality.
        /// </summary>
        public byte Sort => sort;
    }
}