using System;
using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The user module controller.
    /// </summary>
    ///
    public class UserController : AbstController, IMgmt
    {
        ConcurrentDictionary<string, string> vcodes = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 4, 1024);

        public UserController(WebArg arg) : base(arg)
        {
            SetMuxer<UserMuxer>();
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <code>
        /// GET /user/new?id=_id_
        ///
        /// POST /user/new
        /// {
        ///   "id" : "_user_id_",            
        ///   "password" : "_password_",            
        ///   "name" : "_name_",            
        ///   "vcode" : "123"            
        /// }    
        /// </code>
        public void @new(WebContext wc, string subscpt)
        {
            if (wc.IsGetMethod)
            {
                string id = null;
                if (!wc.Get(nameof(id), ref id))
                {
                    wc.StatusCode = 400; // bad request
                    return;
                }

                string vcode = "123";
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryA("SELECT id FROM users WHERE id = @1", p => p.Put(id)))
                    {
                        wc.StatusCode = 409; // conflict
                    }
                    else
                    {
                        vcodes.TryAdd(id, vcode);
                        wc.StatusCode = 200; // ok
                    }
                }
            }
            else
            {
                JObj jo = wc.ReadJObj();
                string id = jo[nameof(id)];
                string name = jo[nameof(name)];
                string password = jo[nameof(password)];
                string vcode = jo[nameof(vcode)];

                string storedvcode;
                vcodes.TryRemove(id, out storedvcode);

                if (vcode.Equals(storedvcode))
                {
                    using (var dc = Service.NewDbContext())
                    {
                        string credential = StrUtility.MD5(id + ':' + ':' + password);
                        if (dc.Execute("INSERT INTO users (id, credential, name) VALUES (@1, @2, @3)", p => p.Put(id).Put(credential).Put(name)) > 0)
                        {
                            wc.StatusCode = 201; // created
                        }
                        else
                            wc.StatusCode = 500; // internal server error
                    }
                }
                else
                    wc.StatusCode = 400; // bad request
            }
        }




        [CheckAdmin]
        public void create(WebContext wc, string subscpt)
        {
            JObj jo = wc.ReadJObj();
            string id = jo[nameof(id)];
            string password = jo[nameof(password)];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("INSERT INTO users (id, credential) VALUES (@1, @2)", p => p.Put(id).Put(StrUtility.MD5(password))) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 500; // internal error
                }
            }
        }

        //
        // ADMIN
        //

        public void srch(WebContext wc, string subscpt)
        {
        }

        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }
}