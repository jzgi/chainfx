using System;

namespace Greatbone.Core
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AllowAttribute : Attribute
	{
		private string[] roles;

		public AllowAttribute(params string[] roles)
		{
			this.roles = roles;
		}
	}
}