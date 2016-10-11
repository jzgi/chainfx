using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The notice service.</summary>
    ///
    public class NoticeModule : WebModule, IAdmin
    {
        public NoticeModule(ISetting setg) : base(setg)
        {
            SetVarHub<NoticeVarHub>(false);
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void @default(WebContext wc)
        {
            int page = 0;
            wc.Got("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @offset", p => p.Put("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }


        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        public void @new(WebContext wc)
        {
            JObj jo = wc.JObj;
            int age = jo[nameof(age)];


            User obj = wc.Obj<User>();


            int page = 0;
            wc.Got("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("INSERT INTO notices () VALUES ()",
                    p => { p.Put(page * 20); p.Put(page * 20); }))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
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

        public void getimg(WebContext wc)
        {
            int id = 0;
            if (!wc.Got(nameof(id), ref id))
            {
                wc.StatusCode = 304; return;
            }
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT content FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    byte[] bytes = dc.GetBytes();
                    wc.SendBytes(200, bytes);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }

        }

        public void putimg(WebContext wc)
        {
            ArraySegment<byte> bytes = wc.BytesSeg;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("INSERT INTO notices () VALUES (content)", p => p.Put(bytes.Array)) >= 1)
                {

                }
                else
                {
                    wc.StatusCode = 500;
                }
            }

        }
    }
}