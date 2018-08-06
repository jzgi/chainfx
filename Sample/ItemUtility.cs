using System.Linq;
using Greatbone;

namespace Samp
{
    public static class ItemUtility
    {
        public static string[] GetRelatedItems(this Map<string, Item> items, int uid)
        {
            Roll<string> roll = new Roll<string>(16);
            for (int i = 0; i < items.Count; i++)
            {
                var o = items[i];
                if (o.givers != null && o.givers.Any(x => x.uid == uid))
                {
                    roll.Add(o.name);
                }
            }
            return roll.ToArray();
        }
    }
}