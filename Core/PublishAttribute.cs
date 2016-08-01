using System;

namespace Greatbone.Core
{
    public class PublishAttribute : Attribute
    {
        string topic;

        int priority;

        int duration;

        public string Key => topic;
    }
}