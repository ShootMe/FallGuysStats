using LiteDB;

namespace FallGuysStats {
    public class Profiles {
        public ObjectId ID { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int ProfileOrder { get; set; }
        public string LinkedShowId { get; set; }
    }
}