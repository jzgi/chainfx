using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class NoticeVarHub : WebVarHub
    {
        public NoticeVarHub(WebConfig cfg) : base(cfg)
        {
        }

        /// <summary>
        /// Gets a particular notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="id"></param>
        public override void Default(WebContext wc, string id)
        {

            using (var dc = Service.NewSqlContext())
            {
                if (dc.QueryA("SELECT * FROM notices WHERE id = @id", p => p.Put("@id", id)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Deletes the target notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="id"></param>
        public void Del(WebContext wc, string id)
        {
            string userid = wc.Token.Key;

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Execute("DELETE FROM notices WHERE id = @id AND authorid = @userid", p => p
                    .Put("@id", id)
                    .Put("@userid", userid)) > 0)
                {

                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// To join/enlist the current user to the target notice.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="noticeid"></param>
        public void Join(WebContext wc, string noticeid)
        {
            string userid = wc.Token.Key;

            using (var dc = Service.NewSqlContext())
            {
                if (dc.QueryA("SELECT joins FROM notices WHERE id = @id", p => p.Put("@userid", userid)))
                {
                    // parse to list

                    // update back the table
                    List<Join> list = new List<Join>();
                    if (dc.Execute("UPDATE notices SET joins = @joins", _ => _.Put("@joins", list)) > 0)
                    {

                    }

                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
        }

    }
}