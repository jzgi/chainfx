namespace SkyChain.Db
{
    public class MlLayerNeuron : ILayer, INeuron
    {
        public INeuron[] Elements { get; }

        public ILayer PreviousLayer { get; }

        public ILayer NextLayer { get; }

        public void Input()
        {
            throw new System.NotImplementedException();
        }

        public void Output()
        {
            throw new System.NotImplementedException();
        }
    }
}