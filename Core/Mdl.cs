using System;

namespace Greatbone.Core
{

    public struct Tab
    {
        public string Caption { get; set; }

        public Action<HtContent> Panel { get; set; }

    }


    public interface IListItem
    {

    }

    public struct ListItem : IListItem
    {

    }

    public struct Ref 
    {
        
    }

  
}