using Greatbone.Core;

namespace Greatbone
{

    public class CheckSelfAttribute : CheckAttribute
    {

        public CheckSelfAttribute() : base(true) { }


        public override bool Test(WebContext wc)
        {
            return true;
        }

    }

}