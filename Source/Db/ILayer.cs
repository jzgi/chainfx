namespace SkyChain.Db
{
    public interface ILayer
    {
        INeuron[] Elements { get; }

        ILayer PreviousLayer { get; }

        ILayer NextLayer { get; }
    }
}