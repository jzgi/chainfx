using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WeiXinUtility
    {
        public const string WXAUTH = "wxauth";

        public const string APPID = "wxd007f5ad60226953";

        public const string APPSECRET = "7884c01588649198c2e83ea8d08891b6";

        public const string ADDR = "http://shop.144000.tv";

        public const string MCH_ID = "1445565602";

        public const string NONCE_STR = "30A5FE271";

        static Client WweiXinPay;

        public const string KEY = "28165ACAB74C2FBF3B042B5D2F87D274";

        static readonly Client WeiXin = new Client("https://api.weixin.qq.com");

        const string BODY_DESC = "粗粮达人-健康产品";


        static volatile string AccessToken;

        private static bool stop;

        private static readonly Thread Renewer = new Thread(async () =>
        {
            while (!stop)
            {
                JObj jo = await WeiXin.GetAsync<JObj>(null, "/cgi-bin/token?grant_type=client_credential&appid=" + APPID + "&secret=" + APPSECRET);
                string access_token = jo?[nameof(access_token)];
                AccessToken = access_token;

                // suspend for 1 hour
                Thread.Sleep(3600000);
            }
        });

        internal static void InitWweiXinPay(string p12file)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            X509Certificate2 cert = new X509Certificate2(p12file, WeiXinUtility.MCH_ID, X509KeyStorageFlags.MachineKeySet);
            handler.ClientCertificates.Add(cert);
            var myClient = new System.Net.Http.HttpClient(handler);

            WweiXinPay = new Client(handler)
            {
                BaseAddress = new Uri("https://api.mch.weixin.qq.com")
            };

        }
        
        static WeiXinUtility()
        {
            //            renewer.Start();
        }

        public static void Stop()
        {
            stop = true;
        }

        public static async Task<string> PostUnifiedOrderAsync(long orderid, decimal total, string openid, string ip, string notifyurl)
        {
            var temp = "appid=" + APPID +
                       "&body=" + BODY_DESC +
                       "&mch_id=" + MCH_ID +
                       "&nonce_str=" + NONCE_STR +
                       "&notify_url=" + notifyurl +
                       "&openid=" + openid +
                       "&out_trade_no=" + orderid +
                       "&spbill_create_ip=" + ip +
                       "&total_fee=" + ((int)(total * 100)) +
                       "&trade_type=" + "JSAPI" +
                       "&key=" + KEY;


            XmlContent xml = new XmlContent();
            xml.ELEM("xml", null, () =>
            {
                xml.ELEM("appid", APPID);
                xml.ELEM("mch_id", MCH_ID);
                xml.ELEM("nonce_str", NONCE_STR);
                xml.ELEM("body", BODY_DESC);
                xml.ELEM("out_trade_no", orderid);
                xml.ELEM("total_fee", (int)(total * 100));
                xml.ELEM("notify_url", notifyurl);
                xml.ELEM("spbill_create_ip", ip);
                xml.ELEM("trade_type", "JSAPI");
                xml.ELEM("openid", openid);
                xml.ELEM("sign", StrUtility.MD5(temp));
            });
            XElem xe = (await WweiXinPay.PostAsync<XElem>(null, "/pay/unifiedorder", xml)).Y;
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
                return default(Accessor);
            }
            string openid = jo[nameof(openid)];

            return new Accessor(access_token, openid);
        }

        public struct Accessor
        {
            internal readonly string access_token;

            internal readonly string openid;

            internal Accessor(string access_token, string openid)
            {
                this.access_token = access_token;
                this.openid = openid;
            }
        }

        public static async Task<User> GetUserInfoAsync(string access_token, string openid)
        {
            JObj jo = await WeiXin.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN");
            string nickname = jo[nameof(nickname)];
            string city = jo[nameof(city)];
            return new User { wx = openid, name = nickname, city = city };
        }

        static readonly DateTime EPOCH = new DateTime(1970, 1, 1);

        public static IContent BuildPrepayContent(string prepay_id)
        {
            string package = "prepay_id=" + prepay_id;
            string timeStamp = ((int)(DateTime.Now - EPOCH).TotalSeconds).ToString();

            var temp = "appId=" + APPID +
                       "&nonceStr=" + NONCE_STR +
                       "&package=" + package +
                       "&signType=MD5" +
                       "&timeStamp=" + timeStamp +
                       "&key=" + KEY;

            JsonContent cont = new JsonContent();
            cont.OBJ(delegate
            {
                cont.Put("appId", APPID);
                cont.Put("timeStamp", timeStamp);
                cont.Put("nonceStr", NONCE_STR);
                cont.Put("package", package);
                cont.Put("signType", "MD5");
                cont.Put("paySign", StrUtility.MD5(temp));
            });
            return cont;
        }

        public static async Task PostTransferAsync()
        {
            // <xml>
            // <mch_appid>wxe062425f740c30d8</mch_appid>
            // <mchid>10000098</mchid>
            // <nonce_str>3PG2J4ILTKCH16CQ2502SI8ZNMTM67VS</nonce_str>
            // <partner_trade_no>100000982014120919616</partner_trade_no>
            // <openid>ohO4Gt7wVPxIT1A9GjFaMYMiZY1s</openid>
            // <check_name>OPTION_CHECK</check_name>
            // <re_user_name>张三</re_user_name>
            // <amount>100</amount>
            // <desc>节日快乐!</desc>
            // <spbill_create_ip>10.2.3.10</spbill_create_ip>
            // <sign>C97BDBACF37622775366F38B629F45E3</sign>
            // </xml>
            XmlContent cont = new XmlContent();
            XElem resp = (await WweiXinPay.PostAsync<XElem>(null, "/mmpaymkttransfers/promotion/transfers", cont)).Y;
        }
    }
}