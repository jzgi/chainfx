using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To specify basic information on user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        public UiAttribute(string label = null, string tip = null)
        {
            this.label = label;
            this.tip = tip ?? label;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A group number
        /// </summary>
        public short Group { get; set; }
    }
}