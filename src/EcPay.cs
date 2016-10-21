using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Security.Cryptography;
using AllPay.Payment.Integration;
namespace EcPay
{
    internal static class MD5Encoder
    {
        private readonly static HashAlgorithm Crypto;

        static MD5Encoder()
        {
            MD5Encoder.Crypto = new MD5CryptoServiceProvider();
        }

        public static string Encrypt(string originalString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(originalString);
            byte[] numArray = MD5Encoder.Crypto.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                stringBuilder.Append(numArray[i].ToString("X").PadLeft(2, '0'));
            }
            return stringBuilder.ToString().ToUpper();
        }
    }
}
namespace EcPay
{
    public class EcPay : EcPayMetadata
    {
        public EcPay() : base() { }

        public IEnumerable<string> Create()//ref Hashtable feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            List<string> strs = new List<string>();
            Dictionary<string, object> keyValues = new System.Collections.Generic.Dictionary<string, object>();
            //放上回應的資料
            //if (feedback == null)
            //{
            //    feedback = new Hashtable();
            //}
            //驗證輸入資料
            //strs.AddRange(ServerValidator.Validate(this));
            //strs.AddRange(ServerValidator.Validate(base.Query));
            if (strs.Count == 0)
            {
                Uri baseUri = new Uri(base.ServiceURL);
                base.ServiceURL = new Uri(baseUri, @"Express/Create").ToString();
                keyValues.Add("MerchantID", base.MerchantID);
                keyValues.Add("MerchantTradeNo", base.Send.MerchantTradeNo);
                keyValues.Add("MerchantTradeDate", base.Send.MerchantTradeDate);
                keyValues.Add("LogisticsType", base.Send.LogisticsType);
                string[] strArrays = base.Send.LogisticsSubType.ToString().Split(new char[] { '\u005F' });
                switch ((int)strArrays.Length)
                {
                    case 1:
                        {
                            keyValues.Add("LogisticsSubType", strArrays[0]);
                            break;
                        }
                    case 2:
                        {
                            keyValues.Add("LogisticsSubType", strArrays[1]);
                            break;
                        }
                }
                keyValues.Add("GoodsAmount", base.Send.GoodsAmount);
                keyValues.Add("GoodsName", base.Send.GoodsName);
                keyValues.Add("SenderName", base.Send.SenderName);
                keyValues.Add("SenderPhone", base.Send.SenderPhone);
                keyValues.Add("SenderCellPhone", base.Send.SenderCellPhone);
                keyValues.Add("ReceiverName", base.Send.ReceiverName);
                keyValues.Add("ReceiverPhone", base.Send.ReceiverPhone);
                keyValues.Add("ReceiverCellPhone", base.Send.ReceiverCellPhone);
                keyValues.Add("ReceiverEmail", base.Send.ReceiverEmail);
                keyValues.Add("TradeDesc", base.Send.TradeDesc);
                keyValues.Add("ServerReplyURL", base.Send.ServerReplyURL);
                if (base.Send.LogisticsType == LogisticsType.Home)
                {
                    keyValues.Add("SenderZipCode", base.SendExtend.SenderZipCode);
                    keyValues.Add("SenderAddress", base.SendExtend.SenderAddress);
                    keyValues.Add("ReceiverZipCode", base.SendExtend.ReceiverZipCode);
                    keyValues.Add("ReceiverAddress", base.SendExtend.ReceiverAddress);
                    keyValues.Add("Temperature", string.Format("{0:0000}", (int)base.SendExtend.Temperature));
                    keyValues.Add("Distance", string.Format("{0:00}", (int)base.SendExtend.Distance));
                    keyValues.Add("Specification", string.Format("{0:0000}", (int)base.SendExtend.Specification));
                    keyValues.Add("ScheduledPickupTime", string.Format("{0:0}", (int)base.SendExtend.ScheduledPickupTime));
                    keyValues.Add("ScheduledDeliveryTime", string.Format("{0:0}", (int)base.SendExtend.ScheduledDeliveryTime));
                    keyValues.Add("ScheduledDeliveryDate", string.Format("{0:yyyy/MM/dd}", base.SendExtend.ScheduledDeliveryDate));
                    keyValues.Add("PackageCount", base.SendExtend.PackageCount);
                }
                str = genKeyValue(keyValues);
                empty1 = this.BuildCheckMacValue(str);

                str = string.Concat(str, this.BuildParamenter("CheckMacValue", empty1));
                str = str.Substring(1);
                // DateTime now = DateTime.Now;
                // Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.QueryTradeInfo: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
                if (base.ServiceMethod == HttpMethod.ServerPOST)
                {
                    empty = this.ServerPost(str);
                }
                else //if (base.ServiceMethod != HttpMethod.HttpSOAP)
                {
                    strs.Add("No service for HttpPOST and HttpGET.");
                }
                if (!string.IsNullOrEmpty(empty))
                {
                    if (!empty.StartsWith("1"))
                        strs.Add("Error");
                }
                //檢查回傳資料
                // DateTime dateTime = DateTime.Now;
                // Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.QueryTradeInfo: {1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), empty));
                //if (!string.IsNullOrEmpty(empty))
                //{
                //    str = string.Empty;
                //    empty1 = string.Empty;
                //    string[] strArrays = empty.Split(new char[] { '&' });
                //    for (int i = 0; i < (int)strArrays.Length; i++)
                //    {
                //        string str1 = strArrays[i];
                //        if (!string.IsNullOrEmpty(str1))
                //        {
                //            string[] strArrays1 = str1.Split(new char[] { '=' });
                //            string str2 = strArrays1[0];
                //            string str3 = strArrays1[1];
                //            if (str2 == "CheckMacValue")
                //            {
                //                empty1 = str3;
                //            }
                //            else
                //            {
                //                str = string.Concat(str, string.Format("&{0}={1}", str2, str3));
                //                if (str2 == "PaymentType")
                //                {
                //                    str3 = str3.Replace("_CVS", string.Empty);
                //                    str3 = str3.Replace("_BARCODE", string.Empty);
                //                    str3 = str3.Replace("_Alipay", string.Empty);
                //                    str3 = str3.Replace("_Tenpay", string.Empty);
                //                    str3 = str3.Replace("_CreditCard", string.Empty);
                //                }
                //                if (str2 == "PeriodType")
                //                {
                //                    str3 = str3.Replace("Y", "Year");
                //                    str3 = str3.Replace("M", "Month");
                //                    str3 = str3.Replace("D", "Day");
                //                }
                //                feedback.Add(str2, str3);
                //            }
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(empty1))
                //    {
                //        strs.AddRange(this.CompareCheckMacValue(str, empty1));
                //    }
                //    else
                //    {
                //        strs.Add(string.Format("ErrorCode: {0}", feedback["TradeStatus"]));
                //    }
                //}
            }
            return strs;
        }


        public void PrintTradeDocument(ref string html)
        {
            string queryString = string.Empty;
            string checkMacValue = string.Empty;
            Dictionary<string, object> keyValues = new System.Collections.Generic.Dictionary<string, object>();

            Uri baseUri = new Uri(base.ServiceURL);
            base.ServiceURL = new Uri(baseUri, @"helper/printTradeDocument").ToString();
            keyValues.Add("MerchantID", base.MerchantID);
            keyValues.Add("AllpayLogisticsID", base.Send.AllpayLogisticsID);
            queryString = genKeyValue(keyValues);
            checkMacValue = this.BuildCheckMacValue(queryString);

            queryString = string.Concat(queryString, this.BuildParamenter("CheckMacValue", checkMacValue));
            queryString = queryString.Substring(1);
            // DateTime now = DateTime.Now;
            // Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.QueryTradeInfo: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
            if (base.ServiceMethod == HttpMethod.ServerPOST)
            {
                html = this.ServerPost(queryString);
            }


        }

        private string ServerPost(string parameters)
        {
            string empty = string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(parameters);
            WebRequest length = WebRequest.Create(base.ServiceURL);
            length.ContentType = "application/x-www-form-urlencoded";
            length.Method = "POST";
            length.ContentLength = (long)((int)bytes.Length);
            using (Stream requestStream = length.GetRequestStream())
            {
                requestStream.Write(bytes, 0, (int)bytes.Length);
                requestStream.Close();
            }
            WebResponse response = null;
            try
            {
                response = length.GetResponse();
            }
            catch (Exception ex)
            {

            }
            if (response != null)
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    empty = streamReader.ReadToEnd().Trim();
                }
            }
            response.Close();
            response = null;
            length = null;
            return empty;
        }
        private string BuildCheckMacValue(string parameters)
        {
            string empty = string.Empty;
            empty = string.Format("HashKey={0}{1}&HashIV={2}", base.HashKey, parameters, base.HashIV);
            return MD5Encoder.Encrypt(HttpUtility.UrlEncode(empty).ToLower());
        }

        private string BuildParamenter(string id, object value)
        {
            string empty = string.Empty;
            string str = string.Empty;
            if (value != null)
            {
                empty = (!value.GetType().Equals(typeof(DateTime)) ? value.ToString() : ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss"));
            }
            return string.Format("&{0}={1}", id, empty);
        }


        private string genKeyValue(Dictionary<string, object> keyValues)
        {
            string str = string.Empty;
            keyValues
                .OrderBy(x => x.Key).ToList()
                .ForEach(x =>
                {
                    str = string.Concat(str, BuildParamenter(x.Key, x.Value));
                });
            return str;

        }
    }
}
