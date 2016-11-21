namespace Greatbone.Core
{

    public class NetConfig : IData
    {
        public string sub;

        public string intraddr;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(sub), ref sub);
            s.Get(nameof(intraddr), ref intraddr);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(sub), sub);
            s.Put(nameof(intraddr), intraddr);
        }

        //
        // methods
        

    }

}