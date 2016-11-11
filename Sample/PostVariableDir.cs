using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    public class PostVariableDir : WebDir, IVariable
    {
        public PostVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        ///
        /// Get the record.
        ///
        /// <code>
        /// GET /post/_id_/
        /// </code>
        ///
        public void @default(WebContext wc)
        {
            int id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0 ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, z)._("FROM posts WHERE id = @1");
                if (dc.QueryA(sql.ToString(), p => p.Put(id)))
                {
                    Post obj = dc.ToData<Post>(z);
                    wc.SendJson(200, obj, z);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// Get the nth image.
        ///
        /// <code>
        /// GET /post/_id_/img[-_idx_]
        /// </code>
        ///
        public void img(WebContext wc, int idx)
        {
            int id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + idx + " FROM posts WHERE id = @1", p => p.Put(id)))
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

        ///
        /// Update the nth image.
        ///
        /// <code>
        /// POST /post/_id_/updimg-[_idx_]
        /// [img_bytes]
        /// </code>
        ///
        [Check]
        public void updimg(WebContext wc, int idx)
        {
            int id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.ReadByteA();
                if (bytes == null)
                {
                    wc.StatusCode = 301;
                }
                else if (dc.Execute("UPDATE posts SET m" + idx + " = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
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
        /// Delete the record.
        ///
        /// <code>
        /// POST /post/_id_/del
        /// </code>
        ///
        [Check]
        public void del(WebContext wc)
        {
            int id = wc.Var;
            IPrincipal tok = wc.Principal;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM posts WHERE id = @1 AND authorid = @2", p => p.Put(id).Put(tok.Key)) >
                    0)
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
        /// Add a comment to the record.
        ///
        /// <code>
        /// POST /post/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        ///
        [Check]
        public void cmt(WebContext wc)
        {
            int id = wc.Var;
            IPrincipal tok = wc.Principal;
            var comment = wc.ReadData<Comment>();

            comment.time = DateTime.Now;
            comment.authorid = tok.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT comments FROM posts WHERE id = @1", p => p.Put(id)))
                {
                    Comment[] cmts = dc.GetDatas<Comment>().Add(comment);
                    if (dc.Execute("UPDATE posts SET comments = @1 WHERE id = @2", p => p.Put(cmts).Put(id)) > 0)
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
        /// Increase the shared number of this record.
        ///
        /// <code>
        /// POST /post/_id_/share
        /// </code>
        ///
        public void share(WebContext wc)
        {
            int id = wc.Var;
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
        /// Add current user into the likes array.
        ///
        /// <code>
        /// POST /post/_id_/like
        /// </code>
        ///
        [Check]
        public void like(WebContext wc)
        {
            string uid = wc.Principal.Key;
            int id = wc.Var;
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