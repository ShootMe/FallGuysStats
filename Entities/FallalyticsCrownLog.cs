using System;
using LiteDB;

namespace FallGuysStats {
    public class FallalyticsCrownLog {
        [BsonId(true)]
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string ShowId { get; set; }
        public string RoundId { get; set; }
        public DateTime End { get; set; }
        public string CountryCode { get; set; }
        public int OnlineServiceType { get; set; }
        public string OnlineServiceId { get; set; }
        public string OnlineServiceNickname { get; set; }
        public bool IsTransferSuccess { get; set; }
    }
}