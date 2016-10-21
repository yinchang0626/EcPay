# EcPay
EcPay物流.Net Api

因為官方沒有提供.Net SDK，所以自己造輪子，
可是請注意
- 目前還在開發中，請勿用在正式環境，後果就是自己找bug..... 
- 因為專案很趕，程式碼還很亂，先完成產生黑貓的單和列印，不過各式各樣的防呆程式碼還沒補上
- 官方的說明文件(https://www.ecpay.com.tw/Content/files/ecpay_030.pdf)
- 官方文件沒有程式碼範例，所以讓大家可以參考一下，尤其是CheckMacValue的編碼方式可以不用再自己寫
- 歡迎大家一起補強

使用範例-產生物流單
```sh
           EcPay.EcPay ecpay = new EcPay.EcPay();
            ecpay.ServiceMethod = AllPay.Payment.Integration.HttpMethod.ServerPOST;
            ecpay.MerchantID = "2000132";
            ecpay.HashKey = "5294y06JbISpM5x9";
            ecpay.HashIV = "v77hoKGq4kWxNNIS";
            ecpay.ServiceURL = "https://logistics-stage.ecpay.com.tw";

            ecpay.Send.MerchantTradeNo = "YC201610210002";
            ecpay.Send.MerchantTradeDate = DateTime.Now;
            ecpay.Send.LogisticsType = EcPay.LogisticsType.Home;
            ecpay.Send.LogisticsSubType = EcPay.LogisticsSubType.Home_TCAT;
            ecpay.Send.GoodsAmount = 5000;
            ecpay.Send.GoodsName = "西瓜";
            ecpay.Send.SenderName = "西瓜人";
            ecpay.Send.SenderPhone = "043807983";
            ecpay.Send.SenderCellPhone = "0953000000";
            ecpay.Send.ReceiverName = "吃西瓜";
            ecpay.Send.ReceiverPhone = "043807983";
            ecpay.Send.ReceiverCellPhone = "0953000000";
            ecpay.Send.ReceiverEmail = "yinchang0626@gmail.com";
            ecpay.Send.TradeDesc = "西瓜很重";
            ecpay.Send.ServerReplyURL = "http://localhost:/OO";
            ecpay.Send.ClientReplyURL = "http://localhost:/OO";
            ecpay.SendExtend.SenderZipCode = "402";
            ecpay.SendExtend.SenderAddress = "台中市";
            ecpay.SendExtend.ReceiverZipCode = "402";
            ecpay.SendExtend.ReceiverAddress = "台中市";
            ecpay.SendExtend.Temperature = EcPay.Temperature.常溫;
            ecpay.SendExtend.Distance = EcPay.Distance.同縣市;
            ecpay.SendExtend.Specification = EcPay.Specification._60cm;
            ecpay.SendExtend.ScheduledPickupTime = EcPay.ScheduledPickupTime._9_12;
            ecpay.SendExtend.ScheduledDeliveryTime = EcPay.ScheduledDeliveryTime._9_12;
            ecpay.SendExtend.ScheduledDeliveryDate = DateTime.Now.AddDays(3.0d);
            ecpay.SendExtend.PackageCount = 1;

            var result=ecpay.Create();
```
使用範例-列印物流單
```sh
            EcPay.EcPay ecpay = new EcPay.EcPay();
            ecpay.ServiceMethod = AllPay.Payment.Integration.HttpMethod.ServerPOST;
            ecpay.MerchantID = "2000132";
            ecpay.HashKey = "5294y06JbISpM5x9";
            ecpay.HashIV = "v77hoKGq4kWxNNIS";
            ecpay.ServiceURL = "https://logistics-stage.ecpay.com.tw";
            ecpay.Send.AllpayLogisticsID = "16493";
            string html = null;
            ecpay.PrintTradeDocument(ref html);
```
