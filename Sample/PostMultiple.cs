using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public class PostMultiple : WebMultiple
    {
        public PostMultiple(WebArg arg) : base(arg)
        {
        }

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
                const byte z = 0 ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, z)._("FROM posts WHERE id = @1");
                if (dc.QueryA(sql.ToString(), p => p.Put(id)))
                {
                    Post obj = dc.ToObj<Post>(z);
                    wc.SendJ(200, obj, z);
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
                    byte[] v = dc.GetBytes();
                    StaticContent sta = new StaticContent() { ByteBuffer = v };
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
        [Check]
        public void updimg(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.ReadBytesSeg();
                if (bytes == null)
                {
                    wc.StatusCode = 301;
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
        [Check]
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
        [Check]
        public void cmt(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            IPrincipal tok = wc.Principal;
            Comment c = wc.ReadObj<Comment>();

            c.time = DateTime.Now;
            c.authorid = tok.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT comments FROM posts WHERE id = @1", p => p.Put(id)))
                {
                    Comment[] arr = dc.GetArr<Comment>().Add(c);
                    if (dc.Execute("UPDATE posts SET comments = @1 WHERE id = @2", p => p.Put(arr).Put(id)) > 0)
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
        [Check]
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