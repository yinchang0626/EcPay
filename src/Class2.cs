using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace AllPay.Payment.Integration
{
    public class AllInOneTest : AllInOneMetadata, IDisposable
    {
        private Page _currentPage;

        private HtmlHead _currentHead;

        private HtmlForm _currentForm;

        private HtmlForm CurrentForm
        {
            get
            {
                if (this._currentForm == null)
                {
                    this._currentForm = this.CurrentPage.Form;
                }
                return this._currentForm;
            }
            set
            {
                this._currentForm = value;
            }
        }

        private HtmlHead CurrentHead
        {
            get
            {
                if (this._currentHead == null)
                {
                    this._currentHead = this.CurrentPage.Header;
                }
                return this._currentHead;
            }
            set
            {
                this._currentHead = value;
            }
        }

        private Page CurrentPage
        {
            get
            {
                if (this._currentPage == null && HttpContext.Current != null)
                {
                    this._currentPage = HttpContext.Current.Handler as Page;
                }
                return this._currentPage;
            }
            set
            {
                this._currentPage = value;
            }
        }

        public AllInOneTest()
        {
        }

        public IEnumerable<string> AioChargeback(ref Hashtable feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            List<string> strs = new List<string>();
            if (feedback == null)
            {
                feedback = new Hashtable();
            }
            strs.AddRange(ServerValidator.Validate(this));
            strs.AddRange(ServerValidator.Validate(base.ChargeBack));
            if (strs.Count == 0)
            {
                str = string.Concat(str, this.BuildParamenter("ChargeBackTotalAmount", base.ChargeBack.ChargeBackTotalAmount));
                str = string.Concat(str, this.BuildParamenter("MerchantID", base.MerchantID));
                str = string.Concat(str, this.BuildParamenter("MerchantTradeNo", base.ChargeBack.MerchantTradeNo));
                str = string.Concat(str, this.BuildParamenter("Remark", base.ChargeBack.Remark));
                str = string.Concat(str, this.BuildParamenter("TradeNo", base.ChargeBack.TradeNo));
                empty1 = this.BuildCheckMacValue(str);
                str = string.Concat(str, this.BuildParamenter("CheckMacValue", empty1));
                str = str.Substring(1);
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.AioChargeback: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
                if (base.ServiceMethod != HttpMethod.ServerPOST)
                {
                    strs.Add("No service for HttpPOST, HttpGET and HttpSOAP.");
                }
                else
                {
                    empty = this.ServerPost(str);
                }
                DateTime dateTime = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.AioChargeback: {1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), empty));
                string[] strArrays = empty.Split(new char[] { '|' });
                if ((int)strArrays.Length == 2)
                {
                    string str1 = strArrays[0];
                    string str2 = strArrays[1];
                    feedback.Add("RtnCode", str1);
                    feedback.Add("RtnMsg", str2);
                }
                if (empty != "1|OK")
                {
                    if (empty.Length <= 2)
                    {
                        strs.Add("Feedback message error!");
                    }
                    else
                    {
                        strs.Add(empty.Substring(2).Replace("-", ": "));
                    }
                }
            }
            return strs;
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

        public IEnumerable<string> CheckOut()
        {
            return this.CheckOut("_self");
        }

        public IEnumerable<string> CheckOut(string target)
        {
            List<string> strs = new List<string>();
            strs.AddRange(ServerValidator.Validate(this));
            strs.AddRange(ServerValidator.Validate(base.Send));
            strs.AddRange(ServerValidator.Validate(base.SendExtend));
            if (strs.Count == 0)
            {
                this.ClearPageControls();
                if (base.Send.ChoosePayment == PaymentMethod.Credit && base.Send.DeviceSource == DeviceType.Mobile && !base.SendExtend.PeriodAmount.HasValue)
                {
                    base.Send.ChoosePayment = PaymentMethod.ALL;
                    base.Send.IgnorePayment = "WebATM#ATM#CVS#BARCODE#Alipay#Tenpay#TopUpUsed#APPBARCODE#AccountLink";
                }
                string str = base.Send.ChoosePayment.ToString();
                string empty = string.Empty;
                string empty1 = string.Empty;
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("AlipayItemCounts", base.SendExtend.AlipayItemCounts));
                }
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("AlipayItemName", base.SendExtend.AlipayItemName));
                }
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("AlipayItemPrice", base.SendExtend.AlipayItemPrice));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("ChoosePayment", str));
                if (base.Send.ChooseSubPayment != PaymentMethodItem.None)
                {
                    string[] strArrays = base.Send.ChooseSubPayment.ToString().Split(new char[] { '\u005F' });
                    switch ((int)strArrays.Length)
                    {
                        case 1:
                            {
                                empty = string.Concat(empty, this.RenderControlAndParamenter("ChooseSubPayment", strArrays[0]));
                                break;
                            }
                        case 2:
                            {
                                empty = string.Concat(empty, this.RenderControlAndParamenter("ChooseSubPayment", strArrays[1]));
                                break;
                            }
                    }
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("ClientBackURL", base.Send.ClientBackURL));
                if ((str == "ATM" || str == "CVS" || str == "BARCODE") && !string.IsNullOrEmpty(base.SendExtend.ClientRedirectURL))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("ClientRedirectURL", base.SendExtend.ClientRedirectURL));
                }
                if ((str == "ALL" || str == "Credit") && base.SendExtend.CreditInstallment.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("CreditInstallment", base.SendExtend.CreditInstallment));
                }
                if (str == "CVS" || str == "BARCODE")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Desc_1", base.SendExtend.Desc_1));
                }
                if (str == "CVS" || str == "BARCODE")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Desc_2", base.SendExtend.Desc_2));
                }
                if (str == "CVS" || str == "BARCODE")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Desc_3", base.SendExtend.Desc_3));
                }
                if (str == "CVS" || str == "BARCODE")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Desc_4", base.SendExtend.Desc_4));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("DeviceSource", base.Send.DeviceSource.ToString().Substring(0, 1)));
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Email", base.SendExtend.Email));
                }
                if (str == "ATM")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("ExpireDate", base.SendExtend.ExpireDate));
                }
                if (str == "Tenpay")
                {
                    DateTime expireTime = base.SendExtend.ExpireTime;
                    empty = string.Concat(empty, this.RenderControlAndParamenter("ExpireTime", expireTime.ToString("yyyy/MM/dd HH:mm:ss")));
                }
                if (str == "Credit" && base.SendExtend.ExecTimes.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("ExecTimes", base.SendExtend.ExecTimes));
                }
                if (str == "Credit" && base.SendExtend.Frequency.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Frequency", base.SendExtend.Frequency));
                }
                if (!string.IsNullOrEmpty(base.Send.IgnorePayment))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("IgnorePayment", base.Send.IgnorePayment));
                }
                if ((str == "ALL" || str == "Credit") && base.SendExtend.InstallmentAmount.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("InstallmentAmount", base.SendExtend.InstallmentAmount));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("ItemName", base.Send.ItemName));
                empty = string.Concat(empty, this.RenderControlAndParamenter("ItemURL", base.Send.ItemURL));
                if (!string.IsNullOrEmpty(base.SendExtend.Language))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Language", base.SendExtend.Language));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("MerchantID", base.MerchantID));
                empty = string.Concat(empty, this.RenderControlAndParamenter("MerchantTradeDate", base.Send.MerchantTradeDate));
                empty = string.Concat(empty, this.RenderControlAndParamenter("MerchantTradeNo", base.Send.MerchantTradeNo));
                empty = string.Concat(empty, this.RenderControlAndParamenter("NeedExtraPaidInfo", base.Send.NeedExtraPaidInfo.ToString().Substring(0, 1)));
                empty = string.Concat(empty, this.RenderControlAndParamenter("OrderResultURL", base.Send.OrderResultURL));
                if ((str == "ALL" || str == "ATM" || str == "CVS" || str == "BARCODE") && !string.IsNullOrEmpty(base.SendExtend.PaymentInfoURL))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PaymentInfoURL", base.SendExtend.PaymentInfoURL));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("PaymentType", base.Send.PaymentType));
                if (str == "Credit" && base.SendExtend.PeriodAmount.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PeriodAmount", base.SendExtend.PeriodAmount));
                }
                if (str == "Credit" && !string.IsNullOrEmpty(base.SendExtend.PeriodReturnURL))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PeriodReturnURL", base.SendExtend.PeriodReturnURL));
                }
                if (str == "Credit" && base.SendExtend.PeriodType != PeriodType.None)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PeriodType", base.SendExtend.PeriodType.ToString().Substring(0, 1)));
                }
                if (base.Send.PlatformChargeFee.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PlatformChargeFee", base.Send.PlatformChargeFee));
                }
                if (!string.IsNullOrEmpty(base.Send.PlatformID))
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PlatformID", base.Send.PlatformID));
                }
                if (base.Send.InvoiceMark == InvoiceState.Yes)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("InvoiceMark", "Y"));
                }
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("PhoneNo", base.SendExtend.PhoneNo));
                }
                if ((str == "ALL" || str == "Credit") && base.SendExtend.Redeem.HasValue && base.SendExtend.Redeem.Value)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("Redeem", "Y"));
                }
                empty = string.Concat(empty, this.RenderControlAndParamenter("Remark", base.Send.Remark));
                empty = string.Concat(empty, this.RenderControlAndParamenter("ReturnURL", base.Send.ReturnURL));
                empty = string.Concat(empty, this.RenderControlAndParamenter("TotalAmount", base.Send.TotalAmount));
                empty = string.Concat(empty, this.RenderControlAndParamenter("TradeDesc", base.Send.TradeDesc));
                if ((str == "ALL" || str == "Credit") && base.SendExtend.UnionPay.HasValue)
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("UnionPay", (base.SendExtend.UnionPay.Value ? 1 : 0)));
                }
                if (str == "Alipay")
                {
                    empty = string.Concat(empty, this.RenderControlAndParamenter("UserName", base.SendExtend.UserName));
                }
                empty1 = this.BuildCheckMacValue(empty);
                empty = string.Concat(empty, this.RenderControlAndParamenter("CheckMacValue", empty1));
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.CheckOut: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), empty));
                if (base.ServiceMethod != HttpMethod.HttpPOST)
                {
                    strs.Add("No service for HttpGET, ServerPOST and HttpSOAP.");
                }
                else if (this.CurrentPage != null)
                {
                    HtmlGenericControl htmlGenericControl = new HtmlGenericControl("script");
                    htmlGenericControl.Attributes.Add("type", "text/javascript");
                    htmlGenericControl.Attributes.Add("src", this.CurrentPage.ClientScript.GetWebResourceUrl(base.GetType(), "AllPay.Payment.Integration.Resources.jquery-1.4.1.min.js"));
                    HtmlGenericControl htmlGenericControl1 = new HtmlGenericControl("script");
                    htmlGenericControl1.Attributes.Add("type", "text/javascript");
                    htmlGenericControl1.InnerHtml = string.Empty;
                    HtmlGenericControl htmlGenericControl2 = htmlGenericControl1;
                    htmlGenericControl2.InnerHtml = string.Concat(htmlGenericControl2.InnerHtml, "\r\n//<![CDATA[");
                    HtmlGenericControl htmlGenericControl3 = htmlGenericControl1;
                    htmlGenericControl3.InnerHtml = string.Concat(htmlGenericControl3.InnerHtml, "\r\n    $(document).ready(function () {");
                    HtmlGenericControl htmlGenericControl4 = htmlGenericControl1;
                    htmlGenericControl4.InnerHtml = string.Concat(htmlGenericControl4.InnerHtml, "\r\n        $(\".__parameter\").each(function (i) {");
                    HtmlGenericControl htmlGenericControl5 = htmlGenericControl1;
                    htmlGenericControl5.InnerHtml = string.Concat(htmlGenericControl5.InnerHtml, "\r\n            this.name = this.id;");
                    HtmlGenericControl htmlGenericControl6 = htmlGenericControl1;
                    htmlGenericControl6.InnerHtml = string.Concat(htmlGenericControl6.InnerHtml, "\r\n        });");
                    HtmlGenericControl htmlGenericControl7 = htmlGenericControl1;
                    htmlGenericControl7.InnerHtml = string.Concat(htmlGenericControl7.InnerHtml, "\r\n");
                    HtmlGenericControl htmlGenericControl8 = htmlGenericControl1;
                    htmlGenericControl8.InnerHtml = string.Concat(htmlGenericControl8.InnerHtml, "\r\n        $(\"input[type=hidden]:not(.__parameter)\").each(function (i) {");
                    HtmlGenericControl htmlGenericControl9 = htmlGenericControl1;
                    htmlGenericControl9.InnerHtml = string.Concat(htmlGenericControl9.InnerHtml, "\r\n            $(this).remove();");
                    HtmlGenericControl htmlGenericControl10 = htmlGenericControl1;
                    htmlGenericControl10.InnerHtml = string.Concat(htmlGenericControl10.InnerHtml, "\r\n        });");
                    HtmlGenericControl htmlGenericControl11 = htmlGenericControl1;
                    htmlGenericControl11.InnerHtml = string.Concat(htmlGenericControl11.InnerHtml, "\r\n");
                    HtmlGenericControl htmlGenericControl12 = htmlGenericControl1;
                    htmlGenericControl12.InnerHtml = string.Concat(htmlGenericControl12.InnerHtml, string.Format("\r\n        $(\"form\").attr(\"target\", \"{0}\");", target));
                    HtmlGenericControl htmlGenericControl13 = htmlGenericControl1;
                    htmlGenericControl13.InnerHtml = string.Concat(htmlGenericControl13.InnerHtml, "\r\n        $(\"form\").submit();");
                    HtmlGenericControl htmlGenericControl14 = htmlGenericControl1;
                    htmlGenericControl14.InnerHtml = string.Concat(htmlGenericControl14.InnerHtml, "\r\n    });");
                    HtmlGenericControl htmlGenericControl15 = htmlGenericControl1;
                    htmlGenericControl15.InnerHtml = string.Concat(htmlGenericControl15.InnerHtml, "\r\n//]]>");
                    HtmlGenericControl htmlGenericControl16 = htmlGenericControl1;
                    htmlGenericControl16.InnerHtml = string.Concat(htmlGenericControl16.InnerHtml, "\r\n");
                    this.CurrentHead.Controls.Clear();
                    this.CurrentForm.Controls.AddAt(0, htmlGenericControl);
                    this.CurrentForm.Controls.AddAt(1, htmlGenericControl1);
                    this.CurrentForm.Action = base.ServiceURL;
                }
            }
            return strs;
        }

        public IEnumerable<string> CheckOutFeedback(ref Hashtable feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            List<string> strs = new List<string>();
            if (feedback == null)
            {
                feedback = new Hashtable();
            }
            if (this.CurrentPage != null)
            {
                Array.Sort<string>(this.CurrentPage.Request.Form.AllKeys);
                string[] allKeys = this.CurrentPage.Request.Form.AllKeys;
                for (int i = 0; i < (int)allKeys.Length; i++)
                {
                    string str1 = allKeys[i];
                    string item = this.CurrentPage.Request.Form[str1];
                    if (str1 == "CheckMacValue")
                    {
                        str = item;
                    }
                    else
                    {
                        empty = string.Concat(empty, string.Format("&{0}={1}", str1, item));
                        if (str1 == "PaymentType")
                        {
                            item = item.Replace("_CVS", string.Empty);
                            item = item.Replace("_BARCODE", string.Empty);
                            item = item.Replace("_Alipay", string.Empty);
                            item = item.Replace("_Tenpay", string.Empty);
                            item = item.Replace("_CreditCard", string.Empty);
                        }
                        if (str1 == "PeriodType")
                        {
                            item = item.Replace("Y", "Year");
                            item = item.Replace("M", "Month");
                            item = item.Replace("D", "Day");
                        }
                        feedback.Add(str1, item);
                    }
                }
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.CheckOutFeedback: {1}&CheckMacValue={2}", now.ToString("yyyy-MM-dd HH:mm:ss"), empty, str));
                strs.AddRange(this.CompareCheckMacValue(empty, str));
            }
            return strs;
        }

        public IEnumerable<string> CheckOutString(ref string html)
        {
            return this.CheckOutString("_self", ref html);
        }

        public IEnumerable<string> CheckOutString(string target, ref string html)
        {
            List<string> strs = null;
            this.CurrentPage = new Page()
            {
                EnableEventValidation = false
            };
            this.CurrentHead = new HtmlHead()
            {
                Title = "Form Redirect"
            };
            this.CurrentForm = new HtmlForm()
            {
                Name = "aspnetForm"
            };
            strs = new List<string>();
            strs.AddRange(this.CheckOut(target));
            if (strs.Count == 0)
            {
                this.CurrentPage.Controls.Add(this.CurrentHead);
                this.CurrentPage.Controls.Add(this.CurrentForm);
                StringBuilder stringBuilder = new StringBuilder();
                HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter(stringBuilder));
                this.CurrentForm.RenderControl(htmlTextWriter);
                html = stringBuilder.ToString();
            }
            return strs;
        }

        private void ClearPageControls()
        {
            if (this.CurrentPage != null)
            {
                for (int i = this.CurrentForm.Controls.Count; i > 0; i--)
                {
                    Control item = this.CurrentForm.Controls[i - 1];
                    if (item.GetType().Name != "ScriptManager" && item.GetType().Name != "ToolkitScriptManager")
                    {
                        this.CurrentForm.Controls.Remove(item);
                    }
                }
            }
        }

        private IEnumerable<string> CompareCheckMacValue(string parameters, string checkMacValue)
        {
            List<string> strs = new List<string>();
            if (!string.IsNullOrEmpty(checkMacValue))
            {
                if (checkMacValue != this.BuildCheckMacValue(parameters))
                {
                    strs.Add("CheckMacValue verify fail.");
                }
            }
            else if (string.IsNullOrEmpty(checkMacValue))
            {
                strs.Add("No CheckMacValue parameter.");
            }
            return strs;
        }

        public void Dispose()
        {
            GC.Collect();
        }

        public IEnumerable<string> DoAction(ref Hashtable feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            List<string> strs = new List<string>();
            if (feedback == null)
            {
                feedback = new Hashtable();
            }
            strs.AddRange(ServerValidator.Validate(this));
            strs.AddRange(ServerValidator.Validate(base.Action));
            if (strs.Count == 0)
            {
                base.Send.ChoosePayment.ToString();
                str = string.Concat(str, this.BuildParamenter("Action", base.Action.Action.ToString()));
                str = string.Concat(str, this.BuildParamenter("MerchantID", base.MerchantID));
                str = string.Concat(str, this.BuildParamenter("MerchantTradeNo", base.Action.MerchantTradeNo));
                str = string.Concat(str, this.BuildParamenter("TotalAmount", base.Action.TotalAmount));
                str = string.Concat(str, this.BuildParamenter("TradeNo", base.Action.TradeNo));
                empty1 = this.BuildCheckMacValue(str);
                str = string.Concat(str, this.BuildParamenter("CheckMacValue", empty1));
                str = str.Substring(1);
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.DoAction: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
                if (base.ServiceMethod != HttpMethod.ServerPOST)
                {
                    strs.Add("No service for HttpPOST, HttpGET and HttpSOAP.");
                }
                else
                {
                    empty = this.ServerPost(str);
                }
                DateTime dateTime = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.DoAction: {1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), empty));
                if (!string.IsNullOrEmpty(empty))
                {
                    str = string.Empty;
                    empty1 = string.Empty;
                    string[] strArrays = empty.Split(new char[] { '&' });
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = strArrays[i];
                        if (!string.IsNullOrEmpty(str1))
                        {
                            string[] strArrays1 = str1.Split(new char[] { '=' });
                            string str2 = strArrays1[0];
                            string str3 = strArrays1[1];
                            if (str2 == "CheckMacValue")
                            {
                                empty1 = str3;
                            }
                            else
                            {
                                str = string.Concat(str, string.Format("&{0}={1}", str2, str3));
                                feedback.Add(str2, str3);
                            }
                        }
                    }
                    if (feedback.ContainsKey("RtnCode") && !"1".Equals(feedback["RtnCode"]))
                    {
                        strs.Add(string.Format("{0}: {1}", feedback["RtnCode"], feedback["RtnMsg"]));
                    }
                }
            }
            return strs;
        }

        public IEnumerable<string> QueryPeriodCreditCardTradeInfo(ref PeriodCreditCardTradeInfo feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            List<string> strs = new List<string>();
            if (feedback == null)
            {
                feedback = new PeriodCreditCardTradeInfo();
            }
            strs.AddRange(ServerValidator.Validate(this));
            strs.AddRange(ServerValidator.Validate(base.Query));
            if (strs.Count == 0)
            {
                str = string.Concat(str, this.BuildParamenter("MerchantID", base.MerchantID));
                str = string.Concat(str, this.BuildParamenter("MerchantTradeNo", base.Query.MerchantTradeNo));
                str = string.Concat(str, this.BuildParamenter("TimeStamp", base.Query.TimeStamp));
                empty1 = this.BuildCheckMacValue(str);
                str = string.Concat(str, this.BuildParamenter("CheckMacValue", empty1));
                str = str.Substring(1);
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.QueryTradeInfo: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
                if (base.ServiceMethod != HttpMethod.ServerPOST)
                {
                    strs.Add("No service for HttpPOST and HttpGET.");
                }
                else
                {
                    empty = this.ServerPost(str);
                    feedback = (new JavaScriptSerializer()).Deserialize<PeriodCreditCardTradeInfo>(empty);
                }
                DateTime dateTime = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.QueryTradeInfo: {1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), empty));
            }
            return strs;
        }

        public IEnumerable<string> QueryTradeInfo(ref Hashtable feedback)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            List<string> strs = new List<string>();
            if (feedback == null)
            {
                feedback = new Hashtable();
            }
            strs.AddRange(ServerValidator.Validate(this));
            strs.AddRange(ServerValidator.Validate(base.Query));
            if (strs.Count == 0)
            {
                str = string.Concat(str, this.BuildParamenter("MerchantID", base.MerchantID));
                str = string.Concat(str, this.BuildParamenter("MerchantTradeNo", base.Query.MerchantTradeNo));
                str = string.Concat(str, this.BuildParamenter("TimeStamp", base.Query.TimeStamp));
                empty1 = this.BuildCheckMacValue(str);
                str = string.Concat(str, this.BuildParamenter("CheckMacValue", empty1));
                str = str.Substring(1);
                DateTime now = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  OUTPUT  AllInOne.QueryTradeInfo: {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), str));
                if (base.ServiceMethod == HttpMethod.ServerPOST)
                {
                    empty = this.ServerPost(str);
                }
                else if (base.ServiceMethod != HttpMethod.HttpSOAP)
                {
                    strs.Add("No service for HttpPOST and HttpGET.");
                }
                else
                {
                    IAllPayService allPayService = ChannelProvider.CreateChannel<IAllPayService>(base.ServiceURL);
                    empty = allPayService.QueryTradeInfo(base.MerchantID, base.Query.MerchantTradeNo, base.Query.TimeStamp, empty1);
                }
                DateTime dateTime = DateTime.Now;
                Logger.WriteLine(string.Format("INFO   {0}  INPUT   AllInOne.QueryTradeInfo: {1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), empty));
                if (!string.IsNullOrEmpty(empty))
                {
                    str = string.Empty;
                    empty1 = string.Empty;
                    string[] strArrays = empty.Split(new char[] { '&' });
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = strArrays[i];
                        if (!string.IsNullOrEmpty(str1))
                        {
                            string[] strArrays1 = str1.Split(new char[] { '=' });
                            string str2 = strArrays1[0];
                            string str3 = strArrays1[1];
                            if (str2 == "CheckMacValue")
                            {
                                empty1 = str3;
                            }
                            else
                            {
                                str = string.Concat(str, string.Format("&{0}={1}", str2, str3));
                                if (str2 == "PaymentType")
                                {
                                    str3 = str3.Replace("_CVS", string.Empty);
                                    str3 = str3.Replace("_BARCODE", string.Empty);
                                    str3 = str3.Replace("_Alipay", string.Empty);
                                    str3 = str3.Replace("_Tenpay", string.Empty);
                                    str3 = str3.Replace("_CreditCard", string.Empty);
                                }
                                if (str2 == "PeriodType")
                                {
                                    str3 = str3.Replace("Y", "Year");
                                    str3 = str3.Replace("M", "Month");
                                    str3 = str3.Replace("D", "Day");
                                }
                                feedback.Add(str2, str3);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(empty1))
                    {
                        strs.AddRange(this.CompareCheckMacValue(str, empty1));
                    }
                    else
                    {
                        strs.Add(string.Format("ErrorCode: {0}", feedback["TradeStatus"]));
                    }
                }
            }
            return strs;
        }

        private string RenderControlAndParamenter(string id, object value)
        {
            string empty = string.Empty;
            if (value != null)
            {
                empty = (!value.GetType().Equals(typeof(DateTime)) ? value.ToString() : ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss"));
            }
            if (this.CurrentPage != null)
            {
                HtmlInputHidden htmlInputHidden = null;
                htmlInputHidden = new HtmlInputHidden()
                {
                    ID = id,
                    Value = empty
                };
                htmlInputHidden.Attributes["class"] = "__parameter";
                this.CurrentForm.Controls.Add(htmlInputHidden);
            }
            return this.BuildParamenter(id, value);
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
            WebResponse response = length.GetResponse();
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
    }
}