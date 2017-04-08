using Greatbone.Core;
using System.Net;
using System.Threading.Tasks;

namespace Greatbone.Sample
{
    public static class WeiXinUtility
    {
        public const string WXAUTH = "wxauth";

        public const string APPID = "wxd007f5ad60226953";

        public const string APPSECRET = "7884c01588649198c2e83ea8d08891b6";

        public const string ADDR = "http://shop.144000.tv";

        public const string MCH_ID = "1445565602";

        public const string NONCE_STR = "sadfasd2s";

        static readonly Connector WweiXinPay = new Connector("https://api.mch.weixin.qq.com");

        static readonly Connector WeiXin = new Connector("https://api.weixin.qq.com");

        public static async Task<string> PostUnifiedOrderAsync(long orderid, decimal total, string openid, string notifyurl)
        {
            XmlContent xml = new XmlContent();
            xml.ELEM("xml", null, () =>
            {
                xml.ELEM("appid", APPID);
                xml.ELEM("mch_id", MCH_ID);
                xml.ELEM("nonce_str", NONCE_STR);
                xml.ELEM("sign", "");
                xml.ELEM("body", "粗粮达人-健康产品");
                xml.ELEM("out_trade_no", orderid);
                xml.ELEM("total_fee", total);
                xml.ELEM("notify_url", notifyurl);
                xml.ELEM("trade_type", "JSAPI");
                xml.ELEM("openid", openid);
            });
            var rsp = await WweiXinPay.PostAsync(null, "/pay/unifiedorder", xml);
            XElem xe = await rsp.ReadAsync<XElem>();
            string prepay_id = xe.Child(nameof(prepay_id));

            return prepay_id;
        }

        public static void GiveRedirectWeiXinAuthorize(this ActionContext ac)
        {
            string redirect_url = WebUtility.UrlEncode(ADDR + ac.Uri);
            ac.SetHeader("Location", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + APPID + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_userinfo&state=" + WXAUTH + "#wechat_redirect");
            ac.Give(303);
        }

        public static async Task<Accessor> GetAccessorAsync(string code)
        {
            string url = "/sns/oauth2/access_token?appid=" + APPID + "&secret=" + APPSECRET + "&code=" + code + "&grant_type=authorization_code";
            JObj jo = await WeiXin.GetAsync<JObj>(null, url);
            if (jo == null) return default(Accessor);

            string access_token = jo[nameof(access_token)];
            if (access_token == null)
            {
                string errmsg = jo[nameof(errmsg)];
                return default(Accessor);
            }
            string openid = jo[nameof(openid)];

            return new Accessor { access_token = access_token, openid = openid };
        }

        public static async Task<User> GetUserInfoAsync(string access_token, string openid)
        {
            JObj jo = await WeiXin.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN");
            string nickname = jo[nameof(nickname)];
            string city = jo[nameof(city)];
            return new User { wx = openid, nickname = nickname, city = city };
        }

        public struct Accessor
        {
            internal string access_token;

            internal string openid;
        }

        public static IContent PrepayContent(string prepay_id)
        {
            JsonContent cont = new JsonContent();
            cont.OBJ(delegate
            {
                cont.Put("appId", APPID);
                cont.Put("timeStamp", "");
                cont.Put("nonceStr", "");
                cont.Put("package", "");
                cont.Put("signType", "MD5");
                cont.Put("paySign", "");
            });
            return cont;
        }
    }
}