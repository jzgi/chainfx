using System;

namespace ChainBase.Web
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

        /// <summary>
        ///  To specify user interface-related attributes.
        /// </summary>
        /// <param name="label">The label text for the target work or action. It can be Unicode symbols or HTML entities</param>
        /// <param name="tip">A short description of the functionality</param>
        /// <param name="group"></param>
        public UiAttribute(string label = null, string tip = null, byte group = 0)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.@group = @group;
        }

        public string Label => label;

        public string Tip => tip;

        /// <summary>
        /// A number that determines grouping of functions.
        /// </summary>
        public byte Group => group;
    }
}