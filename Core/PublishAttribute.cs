using System;

namespace Greatbone.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class PublishAttribute : Attribute
	{
		string topic;

		bool subtype;

		public PublishAttribute(string topic) : this(topic, false)
		{
		}


		public PublishAttribute(string topic, bool subtype)
		{
			this.topic = topic;
			this.subtype = subtype;
		}
	}
}