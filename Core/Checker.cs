using System;

namespace Greatbone.Core
{
	public delegate bool Checker(IToken token);

	public delegate bool Checker<in TX>(IToken token, TX zone) where TX : IComparable<TX>, IEquatable<TX>;


	public interface ICheck
	{
		bool check(IToken token, string x);
	}
}