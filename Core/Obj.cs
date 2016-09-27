namespace Greatbone.Core
{
    public class Obj
    {
        Roll<Pair> pairs;

        public bool Get(string name, ref int value)
        {
            Pair pair;
            if (pairs.TryGet(name, out pair))
            {
                value = pair.Int();
                return true;
            }
            return false;
        }

        public Value this[string name]
        {
            get
            {
                return null;
            }
        }
    }
}