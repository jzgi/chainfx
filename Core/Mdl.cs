using System;

namespace Greatbone.Core
{

    public struct Tab
    {
        public string Caption { get; set; }

        public Action<MdlHtContent> Panel { get; set; }

    }




    public interface IListItem
    {

    }

    public struct ListItem : IListItem
    {

    }

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class Dialog : Attribute
    {
        readonly int mode;

        public Dialog(int mode)
        {
            this.mode = mode;
        }

    }

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ButtonAttribute : Attribute
    {

        public string ShowDialog { get; set; }

    }
}