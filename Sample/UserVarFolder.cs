using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// This folder contains mainly a user's personal stuffs, including the shopping cart and orders.
    ///
    [Ui("用户")]
    public class UserVarFolder : Folder, IVar
    {
        public UserVarFolder(FolderContext fc) : base(fc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string userid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE buyerid = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(userid).Set(page * 20)))
                {
                    ac.GiveFolderPage(null, 200, dc.ToList<Order>());
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }

        [Ui]
        public void cancel(ActionContext ac)
        {

        }

        [Ui]
        public void profile(ActionContext ac)
        {

        }
    }
}