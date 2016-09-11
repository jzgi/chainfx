using System;

namespace Greatbone.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PublishAttribute : Attribute
    {
        public string Topic { get; }

        public bool Subtype { get; }

        public PublishAttribute(string topic) : this(topic, false)
        {
        }


        public PublishAttribute(string topic, bool subtype)
        {
            this.Topic = topic;
            this.Subtype = subtype;
        }
    }
}