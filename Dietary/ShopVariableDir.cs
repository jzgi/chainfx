using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The shop multiplex directory.
    ///
    public class ShopVariableDir : WebDir, IVariable
    {
        public ShopVariableDir(WebDirContext ctx) : base(ctx)
        {
            // customer personal
            AddChild<MyDir>("my");

            // order functions
            AddChild<OrderDir>("order");
        }

        //
        // user actions
        //

        public void @default(WebContext wc, string subscpt)
        {
        }


        public void menu(WebContext wc, string subscpt)
        {
        }

        public void place(WebContext wc, string subscpt)
        {
        }


        //
        // wechat callbacks
        //
    }
}