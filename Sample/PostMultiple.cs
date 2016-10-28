using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{
    public class PostMultiple : WebMultiple
    {
        public PostMultiple(WebArg arg) : base(arg)
        {
        }


        static string DefaultSql = new DbSql("SELECT ").columnlst(new Post())._("FROM posts WHERE id = @1").ToString();

        ///
        /// <summary>
        /// Get the record.
        /// </summary>
        /// <code>
        /// GET /post/_id_/
        /// </code>
        ///
        public override void @default(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA(DefaultSql, p => p.Put(id)))
                {
                    Post obj = dc.ToObj<Post>(0xff ^ BIN);
                    wc.SendJ(200, obj);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// <summary>
        /// Get the nth image.
        /// </summary>
        /// <code>
        /// GET /post/_id_/img[-_n_]
        /// </code>
        ///
        public void img(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + n + " FROM posts WHERE id = @1", p => p.Put(id)))
                {
                    byte[] v = dc.GotBytes();
                    StaticContent sta = new StaticContent() { Buffer = v };
                    wc.Send(200, sta, true, 60000);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Update the nth image.
        /// </summary>
        /// <code>
        /// POST /post/_id_/updimg-[_n_]
        /// [img_bytes]
        /// </code>
        ///
        public void updimg(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.BytesSeg;
                if (bytes == null)
                {
                    wc.StatusCode = 301; ;
                }
                else if (dc.Execute("UPDATE posts SET m" + n + " = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// <summary>
        /// Delete the record.
        /// </summary>
        /// <code>
        /// POST /post/_id_/del
        /// </code>
        ///
        public void del(WebContext wc, string subscpt)
        {
            IPrincipal tok = wc.Principal;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM posts WHERE id = @1 AND authorid = @2", p => p.Put(subscpt).Put(tok.Key)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }


        ///
        /// <summary>
        /// Add a comment to the record.
        /// </summary>
        /// <code>
        /// POST /post/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        ///
        public void cmt(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            IPrincipal tok = wc.Principal;
            JObj jo = wc.JObj;
            string text = jo[nameof(text)];

            Comment c = new Comment
            {
                time = DateTime.Now,
                authorid = tok.Key,
                text = text
            };

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT comments FROM posts WHERE id = @1", p => p.Put(id)))
                {
                    Comment[] arr = dc.GotArr<Comment>().Add(c);
                    if (dc.Execute("UPDATE posts SET comments = @1 WHERE id = @2 AND authorid = @3", p => p.Put(arr).Put(id).Put(tok.Key)) > 0)
                    {
                        wc.StatusCode = 200;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }

        }

        ///
        /// <summary>
        /// Increase the shared number of this record.
        /// </summary>
        /// <code>
        /// POST /post/_id_/share
        /// </code>
        ///
        public void share(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE posts SET shared = shared + 1 WHERE id = @1", p => p.Put(id)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }

        }

        ///
        /// <summary>
        /// Add current user into the likes array.
        /// </summary>
        /// <code>
        /// POST /post/_id_/like
        /// </code>
        ///
        public void like(WebContext wc, string subscpt)
        {
            string uid = wc.Principal.Key;
            int id = wc.Super.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE posts SET likes = array_prepend(@1, likes) WHERE id = @2 AND array_position(likes, @1) ISNULL", p => p.Put(uid).Put(id)) > 0)
                {
                    wc.StatusCode = 200; // ok
                }
                else
                {
                    wc.StatusCode = 409; // conflict
                }
            }

        }
    }
}