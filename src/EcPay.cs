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
namespace EcPay.Payment.Integration
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
namespace EcPay.Payment.Integration
{
    public class EcPay : EcPayMetadata
    {
        public EcPay() : base() { }
        private List<string> returnError(List<string> ErrorStrs, string messgae)
        {
            ErrorStrs.Add(messgae);
            return ErrorStrs;
        }
        public IEnumerable<string> Create(ref Dictionary<string,string> feedback)
        {
            string serverResult = string.Empty;
            string queryString = string.Empty;
            string checkMacValue = string.Empty;
            List<string> ErrorStrs = new List<string>();
            Dictionary<string, object> keyValues = new System.Collections.Generic.Dictionary<string, object>();
            //放上回應的資料
            if (feedback == null)
            {
                feedback = new Dictionary<string,string>();
            }
            //驗證輸入資料
            //strs.AddRange(ServerValidator.Validate(this));
            //strs.AddRange(ServerValidator.Validate(base.Query));
            if (ErrorStrs.Count > 0) return ErrorStrs;
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
            queryString = genKeyValue(keyValues);
            checkMacValue = this.BuildCheckMacValue(queryString);

            queryString = string.Concat(queryString, this.BuildParamenter("CheckMacValue", checkMacValue));
            queryString = queryString.Substring(1);
            // DateTime now = DateTime.Now;
            // Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.QueryTradeInfo: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
            if (base.ServiceMethod != HttpMethod.ServerPOST)
                return this.returnError(ErrorStrs, "No service for HttpPOST and HttpGET.");
            serverResult = this.ServerPost(queryString);
            if (string.IsNullOrEmpty(serverResult) || !serverResult.StartsWith("1|"))
                return this.returnError(ErrorStrs, string.Format("Server Error:{0}", serverResult ?? ""));


            feedback = serverResult.Substring(2).Split(new char[] { '&' }).ToList().Select(keyValueString =>
            {
                var keyValueStrings = keyValueString.Split(new char[] { '=' });
                return new { Key = keyValueStrings[0], Value = keyValueStrings[1] };
            }).ToDictionary(k => k.Key, v => v.Value);


            return ErrorStrs;
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
