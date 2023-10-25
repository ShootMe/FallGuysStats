using System;
using LiteDB;

namespace FallGuysStats {
    public class FallalyticsPbLog {
        [BsonId(true)]
        public int PbId { get; set; }
        public string RoundId { get; set; }
        public string ShowId { get; set; }
        public double Record { get; set; }
        public DateTime PbDate { get; set; }
        public string CountryCode { get; set; }
        public int OnlineServiceType { get; set; }
        public string OnlineServiceId { get; set; }
        public string OnlineServiceNickname { get; set; }
        public bool IsTransferSuccess { get; set; }
    }
}