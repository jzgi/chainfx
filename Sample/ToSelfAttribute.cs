using Greatbone.Core;

namespace Greatbone
{

    public class ToSelfAttribute : ToAttribute
    {

        public ToSelfAttribute() : base(true) { }


        public override bool Test(WebContext wc)
        {
            return true;
        }

    }

}