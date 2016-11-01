using AllPay.Payment.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcPay.Payment.Integration
{
    public class EcPayMetadata: AllPay.Payment.Integration.CommonMetadata
    {
        public EcPayMetadata()
        {
            Send = new SendArguments();
            SendExtend = new SendExtendArguments();
        }
        public SendArguments Send { get; set; }
        public SendExtendArguments SendExtend { get; set; }
        public class SendArguments 
        {
            public string MerchantTradeNo { get; set; }
            public DateTime MerchantTradeDate { get; set; }
            public LogisticsType LogisticsType { get; set; }
            public LogisticsSubType LogisticsSubType { get; set; }
            public int GoodsAmount { get; set; }
            public string GoodsName { get; set; }

            public string SenderName { get; set; }
            public string SenderPhone { get; set; }
            public string SenderCellPhone { get; set; }

            public string ReceiverName { get; set; }
            public string ReceiverPhone { get; set; }
            public string ReceiverCellPhone { get; set; }
            public string ReceiverEmail { get; set; }

            public string TradeDesc { get; set; }
            public string ServerReplyURL { get; set; }
            public string ClientReplyURL { get; set; }
            public string AllpayLogisticsID { get; set; }
        }

        public class SendExtendArguments
        {
            public string SenderZipCode { get; set; }
            public string SenderAddress { get; set; }
            public string ReceiverZipCode { get; set; }
            public string ReceiverAddress { get; set; }
            public Temperature Temperature { get; set; }
            public Distance Distance { get; set; }
            public Specification Specification { get; set; }
            public ScheduledPickupTime ScheduledPickupTime { get; set; }
            public ScheduledDeliveryTime ScheduledDeliveryTime { get; set; }
            public DateTime ScheduledDeliveryDate { get; set; }
            public int PackageCount { get; set; }



        }
    }
}
