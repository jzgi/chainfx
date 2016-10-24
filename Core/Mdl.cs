using System;

namespace Greatbone.Core
{

    public struct Tab
    {
        public string Caption { get; set; }

        public Action<MdlHtmlContent> Panel { get; set; }

    }


    public interface IListItem
    {

    }

    public struct ListItem : IListItem
    {

    }

  
}