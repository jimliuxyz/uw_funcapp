using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UW.Models.Collections
{
    public enum PNS { gcm, apns }

    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string userId { get; set; }

        public string name { get; set; }

        public string phoneno { get; set; }

        public string avatar { get; set; }
        public string tokenRnd { get; set; }

        public List<CurrencySettings> currencies { get; set; }

        public NtfInfo ntfInfo { get; set; }
    }

    public class CurrencySettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CURRENCY_NAME name { get; set; }
        public int order { get; set; }
        public bool isDefault { get; set; }
        public bool isVisible { get; set; }
    }


    public class NtfInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PNS pns { get; set; } //Push Notification System

        public string pnsRegId { get; set; }

        public string azureRegId { get; set; }
    }
    
}
