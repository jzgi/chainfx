namespace Greatbone.Core
{
	public struct CachePolicy
	{
		public bool? IsPublic { get; }

		public int MaxAge { get; }

		internal CachePolicy(bool? @public, int maxage)
		{
			IsPublic = @public;
			MaxAge = maxage;
		}
	}
}