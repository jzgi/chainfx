using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// WeChat interoperability.
    /// </summary>
    ///
    public abstract class WeChatMultiple : WebMultiple
    {
        readonly Login[] logins;

        public WeChatMultiple(WebConfig cfg) : base(cfg)
        {
            logins = JUtility.FileToArr<Login>(cfg.GetFilePath("$realm.json"));
        }


    }

}