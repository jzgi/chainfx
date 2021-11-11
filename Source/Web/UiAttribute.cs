using System;

namespace SkyChain.Web
{
    /// <summary>
    /// To specify basic user interface-related information for a nodule (work or action) object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        readonly byte group;

        readonly short forkie;

        /// <summary>
        ///  To specify user interface-related attributes.
        /// </summary>
        /// <param name="label">The label text for the target work or action. It can be Unicode symbols or HTML entities</param>
        /// <param name="tip">A short description of the functionality</param>
        /// <param name="group"></param>
        /// <param name="forkie"></param>
        public UiAttribute(string label = null, string tip = null, byte group = 0, short forkie = 0)
        {
            this.label = label;
            this.tip = tip;
            this.group = group;
            this.forkie = forkie;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A number that determines grouping of functions.
        /// </summary>
        public byte Group => group;

        public short Forkie => forkie;
    }
}