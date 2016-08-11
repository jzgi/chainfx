namespace Greatbone.Core
{
	public struct CachePolicy
	{
		public bool? IsShared { get; }

		public int MaxAge { get; }

		internal CachePolicy(bool? shared, int maxage)
		{
			IsShared = shared;
			MaxAge = maxage;
		}
	}
}