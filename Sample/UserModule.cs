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
    public class UserModule : WebModule, IAdmin
    {
        ConcurrentDictionary<string, string> vcodes = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 4, 1024);

        public UserModule(WebArg arg) : base(arg)
        {
            SetMultiple<UserMultiple>(false);
        }

        /// <summary>
        /// Get a verification code through SMS.
        /// </summary>
        /// <code>
        /// GET /user/vcode
        /// </code>
        public void vcode(WebContext wc, string subscpt)
        {
            JObj jo = wc.JObj;
            string id = jo[nameof(id)];
            string password = jo[nameof(password)];
            // send vcode through SMS
            string vcode = "1234";
            vcodes.TryAdd(id, vcode);
            INF(vcode);
            wc.StatusCode = 200;
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <code>
        /// POST /user/new
        /// {
        ///   "id" : "_user_id_",            
        ///   "password" : "_password_",            
        ///   "vcode" : "_verification_code_"            
        /// }    
        /// </code>
        public void @new(WebContext wc, string subscpt)
        {
            JObj jo = wc.JObj;
            string id = jo[nameof(id)];
            string password = jo[nameof(password)];
            string vcode = jo[nameof(vcode)];
            string vold;
            if (vcodes.TryGetValue(id, out vold) && vcode.Equals(vold))
            {
                using (var sc = Service.NewDbContext())
                {
                    if (sc.Execute("INSERT INTO users (id, credential) VALUES (@1, @2)", p => p.Put(id).Put(StrUtility.C16(password))) > 0)
                    {
                        wc.StatusCode = 200;
                    }
                }
            }
            else
            {
                wc.StatusCode = 400; // bad request
            }
        }

        [IfAdmin]
        public void create(WebContext wc, string subscpt)
        {
            JObj jo = wc.JObj;
            string id = jo[nameof(id)];
            string password = jo[nameof(password)];
            using (var sc = Service.NewDbContext())
            {
                if (sc.Execute("INSERT INTO users (id, credential) VALUES (@1, @2)", p => p.Put(id).Put(StrUtility.C16(password))) > 0)
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

        public void search(WebContext wc, string subscpt)
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