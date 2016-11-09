using Greatbone.Core;

namespace Ministry.Dietary
{

    ///
    /// <summary>
    /// The shop multiplex controller.
    /// </summary>
    ///
    public abstract class ShopMux : WebMux
    {

        public ShopMux(WebConfig cfg) : base(cfg)
        {
            // customer personal
            AddChild<MyWork>("my");

            // order functions
            AddChild<OrderWork>("order");
        }

        //
        // user actions
        //

        public override void @default(WebContext wc, string subscpt)
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