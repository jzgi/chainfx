namespace DoChain
{
    public interface IGroupKeyable<K> : IKeyable<K>
    {
        /// <summary>
        /// To determine if it is in the same group as the given object. 
        /// </summary>
        /// <param name="akey"></param>
        /// <returns></returns>
        bool GroupWith(K akey);
    }
}