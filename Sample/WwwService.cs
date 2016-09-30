using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service controller.
    ///
    public class WwwService : WebService
    {
        public WwwService(WebServiceConfig cfg) : base(cfg)
        {
            AddSub<MySub>("my", true);

        }

        ///
        /// <summary>Returns the default HTML page</summary> <summary>
        /// 
        /// </summary>
        /// <param name="wc"></param>
        public override void @default(WebContext wc)
        {
            base.@default(wc);
        }

        public void Home(WebContext wc)
        {
        }

        public void Posts(WebContext wc)
        {
        }

        public void notices(WebContext wc)
        {
        }

        public void Search(WebContext wc)
        {
        }

        public void Contact(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                dc.Begin();

                //				sc.DoNonQuery("inaert", o => o.ToString(););

                // msg

                dc.Commit();
            }

            wc.StatusCode = 200;
        }

        //
        // MESSAGES
        // 

        public void USER_UPD(MsgContext mc)
        {

        }
    }
}