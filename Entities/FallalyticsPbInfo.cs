using System;

namespace FallGuysStats {
    public class FallalyticsPbInfo {
        public string RoundId { get; set; }
        public string ShowNameId { get; set; }
        public double Record { get; set; }
        public DateTime PbDate { get; set; }
        public string Country { get; set; }
        public int OnlineServiceType { get; set; }
        public string OnlineServiceId { get; set; }
        public string OnlineServiceNickname { get; set; }
        public bool IsTransferSuccess { get; set; }
    }
}