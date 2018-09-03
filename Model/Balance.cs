using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UW.Models.Collections
{
    public enum CURRENCY_NAME { CNY, USD, BTC, ETH }

    public class Balance //snapshot
    {
        [JsonProperty(PropertyName = "id")]
        public string ownerId { get; set; }

        public List<BalanceSlot> balances { get; set; }
    }
    public class BalanceSlot
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CURRENCY_NAME name { get; set; }
        public string balance { get; set; }
    }
}
