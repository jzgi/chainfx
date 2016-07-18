using System;

namespace Greatbone.Core
{
    public class Subscribe : IMember
    {
        string topic;

        string filter;

        Action handler;

        Subscribe(string topic, string filter, Action handler)
        {
        }

        public string Key => topic;
    }
}