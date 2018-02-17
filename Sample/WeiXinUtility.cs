using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WeiXinUtility
    {
        public const string WXAUTH = "wxauth";

        static string appid;

        static string appsecret;

        static string mchid;

        static string noncestr;

        static string spbillcreateip;

        static string key;

        public const string WatchRef = "https://mp.weixin.qq.com/mp/profile_ext?action=home&__biz=MzU4NDAxMTAwOQ==&scene=110#wechat_redirect";

        static Client WCPay;

        static readonly Client WeiXin = new Client("https://api.weixin.qq.com");

        static volatile string AccessToken;

        private static bool stop;

        private static readonly Thread Renewer = new Thread(async () =>
        {
            while (!stop)
            {
                JObj jo = await WeiXin.GetAsync<JObj>("/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret, null);
                string access_token = jo?[nameof(access_token)];
                AccessToken = access_token;

                // suspend for 1 hour
                Thread.Sleep(3600000);
            }
        });

        public static void Setup(string weixinfile, bool deploy, string p12file = null)
        {
            // load weixin parameters
            var wx = DataUtility.FileTo<JObj>(weixinfile);
            appid = wx[nameof(appid)];
            appsecret = wx[nameof(appsecret)];
            mchid = wx[nameof(mchid)];
            noncestr = wx[nameof(noncestr)];
            spbillcreateip = wx[nameof(spbillcreateip)];
            key = wx[nameof(key)];
            // start the access token renewer thread
            if (deploy)
            {
                Renewer.Start();
            }

            if (p12file != null)
            {
                // load and init client certificate
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual
                };
                X509Certificate2 cert = new X509Certificate2(p12file, mchid, X509KeyStorageFlags.MachineKeySet);
                handler.ClientCertificates.Add(cert);
                WCPay = new Client(handler)
                {
                    BaseAddress = new Uri("https://api.mch.weixin.qq.com")
                };
            }
        }

        public static void Stop()
        {
            stop = true;
        }

        public static void GiveRedirectWeiXinAuthorize(this WebContext ac, string listenAddr)
        {
            string redirect_url = WebUtility.UrlEncode(listenAddr + ac.Uri);
            ac.SetHeader("Location", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_base&state=" + WXAUTH + "#wechat_redirect");
            ac.Give(303);
        }

        public static async Task<(string access_token, string openid)> GetAccessorAsync(string code)
        {
            string url = "/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + code + "&grant_type=authorization_code";
            JObj jo = await WeiXin.GetAsync<JObj>(url, null);
            if (jo == null)
            {
                return default((string, string));
            }
            string access_token = jo[nameof(access_token)];
            if (access_token == null)
            {
                return default((string, string));
            }
            string openid = jo[nameof(openid)];
            return (access_token, openid);
        }

        public static async Task<User> GetUserInfoAsync(string access_token, string openid)
        {
            JObj jo = await WeiXin.GetAsync<JObj>("/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN", null);
            string nickname = jo[nameof(nickname)];
            string city = jo[nameof(city)];
            return new User {wx = openid, name = nickname, city = city};
        }

        static readonly DateTime EPOCH = new DateTime(1970, 1, 1);

        public static IContent BuildPrepayContent(string prepay_id)
        {
            string package = "prepay_id=" + prepay_id;
            string timeStamp = ((int) (DateTime.Now - EPOCH).TotalSeconds).ToString();
            JObj jo = new JObj
            {
                {"appId", appid},
                {"nonceStr", noncestr},
                {"package", package},
                {"signType", "MD5"},
                {"timeStamp", timeStamp}
            };
            jo.Add("paySign", Sign(jo, "paySign"));
            return jo.Dump();
        }

        public static async Task PostSendAsync(string openid, string text)
        {
            var jc = new JsonContent(true);
            jc.OBJ_();
            jc.Put("touser", openid);
            jc.Put("msgtype", "text");
            jc.OBJ_("text");
            jc.Put("content", text);
            jc._OBJ();
            jc._OBJ();
            await WeiXin.PostAsync<XElem>("/cgi-bin/message/custom/send?access_token=" + AccessToken, jc);
        }

        public static async Task PostSendAsync(string openid, string title, string descr, string url, string picurl = null)
        {
            var jc = new JsonContent(true);
            jc.OBJ_();
            jc.Put("touser", openid);
            jc.Put("msgtype", "news");
            jc.OBJ_("news").ARR_("articles").OBJ_();
            jc.Put("title", title);
            jc.Put("description", descr);
            jc.Put("url", url);
            jc.Put("picurl", picurl);
            jc._OBJ()._ARR()._OBJ();
            jc._OBJ();
            await WeiXin.PostAsync<XElem>("/cgi-bin/message/custom/send?access_token=" + AccessToken, jc);
        }

        public static async Task<string> PostTransferAsync(int id, string openid, string username, decimal cash, string desc)
        {
            XElem x = new XElem("xml")
            {
                {"amount", ((int) (cash * 100)).ToString()},
                {"check_name", "FORCE_CHECK"},
                {"desc", desc},
                {"mch_appid", appid},
                {"mchid", mchid},
                {"nonce_str", noncestr},
                {"openid", openid},
                {"partner_trade_no", id.ToString()},
                {"re_user_name", username},
                {"spbill_create_ip", spbillcreateip}
            };
            string sign = Sign(x);
            x.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/mmpaymkttransfers/promotion/transfers", x.Dump()));
            string return_code = xe.Child(nameof(return_code));
            if ("SUCCESS" == return_code)
            {
                string result_code = xe.Child(nameof(result_code));
                if ("SUCCESS" != result_code)
                {
                    string err_code_des = xe.Child(nameof(err_code_des));
                    return err_code_des;
                }
            }
            else
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }
            return null;
        }

        public static async Task<(string, string)> PostUnifiedOrderAsync(string trade_no, decimal total, string openid, string ip, string notifyurl, string descr)
        {
            XElem x = new XElem("xml")
            {
                {"appid", appid},
                {"body", descr},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"notify_url", notifyurl},
                {"openid", openid},
                {"out_trade_no", trade_no},
                {"spbill_create_ip", ip},
                {"total_fee", ((int) (total * 100)).ToString()},
                {"trade_type", "JSAPI"}
            };
            string sign = Sign(x);
            x.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/unifiedorder", x.Dump()));
            string prepay_id = xe.Child(nameof(prepay_id));
            string err_code = null;
            if (prepay_id == null)
            {
                err_code = xe.Child("err_code");
            }
            return (prepay_id, err_code);
        }

        public static bool Notified(XElem xe, out string out_trade_no, out decimal cash)
        {
            cash = 0;
            out_trade_no = null;
            string appid = xe.Child(nameof(appid));
            string mch_id = xe.Child(nameof(mch_id));
            string nonce_str = xe.Child(nameof(nonce_str));

            if (appid != WeiXinUtility.appid || mch_id != mchid || nonce_str != noncestr) return false;

            string result_code = xe.Child(nameof(result_code));

            if (result_code != "SUCCESS") return false;

            string sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return false;

            int cash_fee = xe.Child(nameof(cash_fee)); // in cent
            cash = ((decimal) cash_fee) / 100;
            out_trade_no = xe.Child(nameof(out_trade_no)); // 商户订单号
            return true;
        }

        public static async Task<decimal> PostOrderQueryAsync(string orderno)
        {
            XElem x = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"out_trade_no", orderno}
            };
            string sign = Sign(x);
            x.Add("sign", sign);
            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/orderquery", x.Dump()));
            sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return 0;

            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS") return 0;

            decimal cash_fee = xe.Child(nameof(cash_fee));

            return cash_fee;
        }

        public static async Task<string> PostRefundAsync(string orderno, decimal total, decimal cash)
        {
            XElem xo = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"op_user_id", mchid},
                {"out_refund_no", orderno},
                {"out_trade_no", orderno},
                {"refund_fee", ((int) (cash * 100)).ToString()},
                {"total_fee", ((int) (total * 100)).ToString()}
            };
            string sign = Sign(xo);
            xo.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/secapi/pay/refund", xo.Dump()));
            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS")
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }
            string result_code = xe.Child(nameof(result_code));
            if (result_code != "SUCCESS")
            {
                string err_code_des = xe.Child(nameof(err_code_des));
                return err_code_des;
            }
            return null;
        }

        public static async Task<string> PostRefundQueryAsync(long orderid)
        {
            XElem xo = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"out_trade_no", orderid.ToString()}
            };
            string sign = Sign(xo);
            xo.Add("sign", sign);
            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/refundquery", xo.Dump()));

            sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return "返回结果签名错误";

            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS")
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }

            string result_code = xe.Child(nameof(result_code));
            if (result_code != "SUCCESS")
            {
                return "退款订单查询失败";
            }

            string refund_status_0 = xe.Child(nameof(refund_status_0));
            if (refund_status_0 != "SUCCESS")
            {
                return refund_status_0 == "PROCESSING" ? "退款处理中" :
                    refund_status_0 == "REFUNDCLOSE" ? "退款关闭" : "退款异常";
            }

            return null;
        }

        static string Sign(XElem xe, string exclude = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0; i < xe.Count; i++)
            {
                XElem child = xe[i];
                // not include the sign field
                if (exclude != null && child.Tag == exclude) continue;
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(child.Tag).Append('=').Append(child.Text);
            }
            sb.Append("&key=").Append(key);
            return StrUtility.MD5(sb.ToString());
        }

        static string Sign(JObj jo, string exclude = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0; i < jo.Count; i++)
            {
                JMbr mbr = jo[i];
                // not include the sign field
                if (exclude != null && mbr.Key == exclude) continue;
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(mbr.Key).Append('=').Append((string) mbr);
            }
            sb.Append("&key=").Append(key);
            return StrUtility.MD5(sb.ToString());
        }
    }
}