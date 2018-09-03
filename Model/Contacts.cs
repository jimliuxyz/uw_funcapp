using System.Collections.Generic;
using Newtonsoft.Json;

namespace UW.Models.Collections
{
    public class Contacts
    {
        [JsonProperty(PropertyName = "id")]
        public string ownerId { get; set; }

        public List<Friend> friends { get; set; }
        public List<string> recent { get; set; }
    }

    public class Friend
    {
        public string userId { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
    }
}
