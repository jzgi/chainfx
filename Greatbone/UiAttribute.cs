using System;

namespace Greatbone
{
    /// <summary>
    /// To specify basic user interface-related information for a nodule (work or action) object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        readonly byte sort;

        /// <summary>
        ///  To specify user interface-related attributes.
        /// </summary>
        /// <param name="label">The label text for the target work or action. It can be Unicode symbols or HTML entities</param>
        /// <param name="tip">A short description of the functionality</param>
        /// <param name="sort"></param>
        public UiAttribute(string label = null, string tip = null, byte sort = 0)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.sort = sort;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A sorting number that indicates a particular set of functions.
        /// </summary>
        public byte Sort => sort;
    }
}