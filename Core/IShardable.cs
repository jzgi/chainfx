namespace Greatbone.Core
{
    /// <summary>
    /// To indicate the origin of the data object.
    /// </summary>
    public interface IShardable
    {
        string Shard { get; set; }
    }
}