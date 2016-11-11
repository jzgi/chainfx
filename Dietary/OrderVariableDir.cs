using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The order multiplex directory under shop.
    ///
    public class OrderVariableDir : WebDir, IVariable
    {
        public OrderVariableDir(WebDirContext ctx) : base(ctx)
        {
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