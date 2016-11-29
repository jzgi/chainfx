using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WeChatUtility
    {

        static readonly WebClient Client = new WebClient("weixin", "http://sh.weixin.com");


        public static string GetAccessToken(string appid, string appsecret)
        {
            return null;
        }

    }
}