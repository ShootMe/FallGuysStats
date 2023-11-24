using System;
using LiteDB;

namespace FallGuysStats {
    public class PersonalBestLog {
        [BsonId(true)]
        public DateTime PbDate { get; set; }
        public string SessionId { get; set; }
        public string ShowId { get; set; }
        public string RoundId { get; set; }
        public double Record { get; set; }
        public bool IsPb { get; set; }
        public string CountryCode { get; set; }
        public int OnlineServiceType { get; set; }
        public string OnlineServiceId { get; set; }
        public string OnlineServiceNickname { get; set; }
    }
}