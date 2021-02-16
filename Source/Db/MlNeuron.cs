using System;

namespace SkyChain.Db
{
    public class MlNeuron<P, R> : MlGraph<P, R> where P : struct, IFeature<P> where R : struct, IOutcome<R>, INeuron
    {
        public MlNeuron(int capacity) : base(capacity)
        {
        }

        public void Input()
        {
            throw new NotImplementedException();
        }

        public void Output()
        {
            throw new NotImplementedException();
        }
    }
}