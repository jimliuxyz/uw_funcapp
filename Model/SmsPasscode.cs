
using System;

namespace UW.Models.Collections
{
    //todo : 預計ttl維持24H 當genCount>=3 且 attemptCount>=3時 則待清除後才能被reset,再度使用
    public class SmsPasscode
    {
        public string id { get; set; }
        public string phoneno { get; set; }
        public string passcode { get; set; }

        public int resendCount { get; set; }   // passcode連續產生的次數
        public int verifyCount { get; set; }   // 當下passcode被嘗試的次數
        public DateTime verifyAvailTime { get; set; }
    }
}