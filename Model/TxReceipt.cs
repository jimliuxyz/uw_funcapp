using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UW.Models.Collections
{
    public enum TxType
    {
        DEPOSIT = 1,
        WITHDRAW = 2,
        TRANSFER = 3,
        EXCHANGE = 4
    }

    public class TxReceipt
    {
        [JsonProperty(PropertyName = "id")]
        public string receiptId { get; set; }
        public bool isParent { get; set; }
        public string parentId { get; set; }    //該收據源自於哪個receiptId
        public string executorId { get; set; }  //執行者的userId
        public string ownerId { get; set; } //該收據屬於userId

        public string senderId { get; set; }    //client端透過id自行找到user資訊
        public string receiverId { get; set; }
        public string message { get; set; }
        public string currency { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime datetime = DateTime.UtcNow;
        public int statusCode { get; set; }
        public string statusMsg { get; set; }

        public TxType txType { get; set; }
        public dynamic txParams { get; set; }
        public dynamic txResult { get; set; }
    }

    public class TxActResult
    {
        public bool outflow { get; set; }    //true:入帳(充值) false:出帳
        public decimal amount { get; set; }
        public decimal balance { get; set; } //snapshot
    }

    public class TxParams // 同時為所有Params的基類
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public string currency { get; set; }
        public decimal amount { get; set; }
    }

    public class ExchangeParams : TxParams
    {
        public string toCurrency { get; set; }
    }
}
