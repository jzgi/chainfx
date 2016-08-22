using System;

namespace Greatbone.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class GrantAttribute : Attribute
	{
		Type topic;

		bool subtype;

		public GrantAttribute(Type topic)
		{
		}
	}
}