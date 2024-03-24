using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FallGuysStats {
    public struct OverallRank {
        public bool found { get; set; }
        public int total { get; set; }
        public List<Player> users { get; set; }
        
        public struct Player {
            public int rank { get; set; }
            public string onlineServiceType { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
            public string id { get; set; }
            public double score { get; set; }
            public double firstPlaces { get; set; }
        }
    }
    
    public struct OverallSummary {
        public int rank { get; set; }
        public string country { get; set; }
        public int gold { get; set; }
        public int silver { get; set; }
        public int bronze { get; set; }
        public int pink { get; set; }
        public int players { get; set; }
    }
    
    public struct LevelRank {
        public bool found { get; set; }
        public int total { get; set; }
        public List<RankInfo> recordholders { get; set; }
        
        public struct RankInfo {
            public int rank { get; set; }
            // public string round { get; set; }
            public double record { get; set; }
            public string show { get; set; }
            public bool isAnonymous { get; set; }
            public DateTime finish { get; set; }
            public string country { get; set; }
            public string onlineServiceType { get; set; }
            public string onlineServiceId { get; set; }
            public string onlineServiceNickname { get; set; }
            public string id { get; set; }
        }
    }

    public class LevelRankInfoConverter : JsonConverter<LevelRank.RankInfo> {
        public override LevelRank.RankInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var recordHolder = new LevelRank.RankInfo();
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return recordHolder;
                }

                if (reader.TokenType == JsonTokenType.PropertyName) {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName) {
                        // case "round":
                        //     recordHolder.round = reader.GetString();
                        //     break;
                        case "record":
                            recordHolder.record = reader.GetDouble();
                            break;
                        case "show":
                            recordHolder.show = reader.GetString();
                            break;
                        case "isAnonymous":
                            recordHolder.isAnonymous = reader.GetBoolean();
                            break;
                        case "finish":
                            recordHolder.finish = TimeZoneInfo.ConvertTimeFromUtc(reader.GetDateTime(), TimeZoneInfo.Local);
                            break;
                        case "country":
                            recordHolder.country = reader.GetString();
                            break;
                        case "user":
                            if (reader.TokenType == JsonTokenType.String) {
                                recordHolder.id = reader.GetString();
                            } else if (reader.TokenType == JsonTokenType.StartObject) {
                                while (reader.Read()) {
                                    if (reader.TokenType == JsonTokenType.EndObject) {
                                        break;
                                    }

                                    if (reader.TokenType == JsonTokenType.PropertyName) {
                                        string userPropertyName = reader.GetString();
                                        reader.Read();
                                        switch (userPropertyName) {
                                            case "onlineServiceType":
                                                recordHolder.onlineServiceType = reader.GetString();
                                                break;
                                            case "onlineServiceId":
                                                recordHolder.onlineServiceId = reader.GetString();
                                                break;
                                            case "onlineServiceNickname":
                                                recordHolder.onlineServiceNickname = reader.GetString();
                                                break;
                                            case "id":
                                                recordHolder.id = reader.GetString();
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, LevelRank.RankInfo value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }
    }
    
    public struct AvailableLevel {
        public bool found { get; set; }
        public List<LevelInfo> leaderboards { get; set; }
        
        public struct LevelInfo {
            public string queryname { get; set; }
            public string[] ids { get; set; }
        }
    }
    
    public class RoundConverter : JsonConverter<AvailableLevel.LevelInfo> {
        public override AvailableLevel.LevelInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            AvailableLevel.LevelInfo levelInfo = new AvailableLevel.LevelInfo();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return levelInfo;
                }

                if (reader.TokenType == JsonTokenType.PropertyName) {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName) {
                        case "_name":
                            levelInfo.queryname = reader.GetString();
                            break;
                        case "data":
                            while (reader.Read()) {
                                if (reader.TokenType == JsonTokenType.EndObject) {
                                    break;
                                }

                                if (reader.TokenType == JsonTokenType.PropertyName) {
                                    string dataPropertyName = reader.GetString();
                                    reader.Read();
                                    if (dataPropertyName == "dataname") {
                                        switch (reader.TokenType) {
                                            case JsonTokenType.String:
                                                levelInfo.ids = new[] { reader.GetString() };
                                                break;
                                            case JsonTokenType.StartArray:
                                                levelInfo.ids = JsonSerializer.Deserialize<string[]>(ref reader);
                                                break;
                                            default:
                                                throw new JsonException();
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, AvailableLevel.LevelInfo value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }
    }
    
    public struct SearchResult {
        public bool found { get; set; }
        public List<Player> users { get; set; }
        
        public struct Player {
            public string onlineServiceType { get; set; }
            public string onlineServiceId { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
            public string id { get; set; }
        }
    }
    
    public struct PlayerStats {
        public bool found { get; set; }
        public PlayerInfo user { get; set; }
        public List<PbInfo> pbs { get; set; }
        public SpeedrunRank speedrunrank { get; set; }
        public CrownLeagueRank crownrank { get; set; }
        
        public struct PlayerInfo {
            public string onlineServiceType { get; set; }
            public string onlineServiceId { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
        }
        
        public struct PbInfo {
            public string round { get; set; }
            public double record { get; set; }
            public string show { get; set; }
            public string session { get; set; }
            public bool isAnonymous { get; set; }
            public string ip { get; set; }
            public DateTime finish { get; set; }
            public string country { get; set; }
            public string user { get; set; }
            public int index { get; set; }
            public int roundTotal { get; set; }
            public string roundDisplayName { get; set; }
            public string roundName { get; set; }
        }
        
        public struct SpeedrunRank {
            public string onlineServiceType { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
            public string id { get; set; }
            public double score { get; set; }
            public int firstPlaces { get; set; }
            public int index { get; set; }
            public int total { get; set; }
        }
        
        public struct CrownLeagueRank {
            public string onlineServiceType { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
            public string id { get; set; }
            public double score { get; set; }
            public int crowns { get; set; }
            public int shards { get; set; }
            public int index { get; set; }
            public int total { get; set; }
        }
    }
    
    public struct WeeklyCrown {
        public bool found { get; set; }
        public int total { get; set; }
        public List<Player> users { get; set; }
        public double year { get; set; }
        public double week { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        
        public struct Player {
            public int rank { get; set; }
            public string onlineServiceType { get; set; }
            public string onlineServiceNickname { get; set; }
            public bool isAnonymous { get; set; }
            public string country { get; set; }
            public string id { get; set; }
            public double score { get; set; }
            public double crowns { get; set; }
            public double shards { get; set; }
            public string period { get; set; }
        }
    }
}