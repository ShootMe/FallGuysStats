using System;
using LiteDB;

namespace FallGuysStats {
    public class ServerConnectionLog {
        [BsonId(true)]
        public string SessionId { get; set; }
        public string ShowId { get; set; }
        public string ServerIp { get; set; }
        public DateTime ConnectionDate { get; set; }
        public string CountryCode { get; set; }
        public int OnlineServiceType { get; set; }
        public string OnlineServiceId { get; set; }
        public string OnlineServiceNickname { get; set; }
        public bool IsNotify { get; set; }
        public bool IsPlaying { get; set; }
    }
}