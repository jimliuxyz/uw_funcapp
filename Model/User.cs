using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public enum CURRENCY_NAME { cny, usd, bitcoin, ether }

public class User
{
    [JsonProperty(PropertyName = "id")]
    public string userId { get; set; }

    public string name { get; set; }

    public string phoneno { get; set; }

    public string avatar { get; set; }

    public List<CurrencySettings> currencies { get; set; }
}

public class CurrencySettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public CURRENCY_NAME name { get; set; }
    public int order { get; set; }
    public bool isDefault { get; set; }
    public bool isVisible { get; set; }
}