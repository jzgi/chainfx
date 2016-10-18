using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public class NoticeModule : WebModule, IAdmin
    {
        public NoticeModule(WebArg arg) : base(arg)
        {
            SetVarHub<NoticeVarHub>(false);
        }

        /// <code>
        /// GET /notice/top?[page=_num_]
        /// </code>
        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @1", p => p.Put(page * 20)))
                {
                    Notice[] arr = dc.ToArr<Notice>();
                    wc.Respond(200, arr);
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }


        ///
        /// <code>
        /// POST /post/new
        /// {
        ///     "commentable" : true 
        ///     "text" : "text content" 
        /// }
        /// </code>
        ///
        public void @new(WebContext wc)
        {
            IToken tok = wc.Token;

            Notice obj = wc.JObj.ToObj<Notice>();
            obj.authorid = tok.Key;
            obj.author = tok.Name;

            using (var dc = Service.NewDbContext())
            {
                object id = dc.Scalar(
                    c => c.INSERT_VALUES("notices", obj).RETURNING("id"),
                    p => obj.Save(p)
                );
                if (id != null)
                {
                    wc.StatusCode = 201;
                    wc.SetLocation(id.ToString());
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }

        public void search(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void del(WebContext wc)
        {
            throw new NotImplementedException();
        }

    }
}