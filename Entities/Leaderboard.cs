using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FallGuysStats {
    public class OverallRank {
        public bool found { get; set; }
        public int total { get; set; }
        public List<OverallRankInfo> users { get; set; }
    }
    
    public class OverallRankInfo {
        public int rank { get; set; }
        public string onlineServiceType { get; set; }
        public string onlineServiceNickname { get; set; }
        public bool isAnonymous { get; set; }
        public string country { get; set; }
        public string id { get; set; }
        public double score { get; set; }
        public double firstPlaces { get; set; }
    }
    
    public class OverallRankInfoConverter : JsonConverter<OverallRankInfo> {
        public override OverallRankInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var overallInfo = new OverallRankInfo();
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return overallInfo;
                }

                if (reader.TokenType == JsonTokenType.PropertyName) {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName) {
                        case "onlineServiceType":
                            overallInfo.onlineServiceType = reader.GetString();
                            break;
                        case "onlineServiceNickname":
                            overallInfo.onlineServiceNickname = reader.GetString();
                            break;
                        case "isAnonymous":
                            overallInfo.isAnonymous = reader.GetBoolean();
                            break;
                        case "country":
                            overallInfo.country = reader.GetString();
                            break;
                        case "id":
                            overallInfo.id = reader.GetString();
                            break;
                        case "score":
                            overallInfo.score = reader.GetDouble();
                            break;
                        case "firstPlaces":
                            overallInfo.firstPlaces = reader.GetDouble();
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, OverallRankInfo value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }
    }
    
    public class LevelRank {
        public bool found { get; set; }
        public int total { get; set; }
        public List<LevelRankInfo> recordholders { get; set; }
    }
    
    public class LevelRankInfo {
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
    }

    public class LevelRankInfoConverter : JsonConverter<LevelRankInfo> {
        public override LevelRankInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var recordHolder = new LevelRankInfo();
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
                                recordHolder.onlineServiceId = reader.GetString();
                                // recordHolder.onlineServiceNickname = $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}";
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

        public override void Write(Utf8JsonWriter writer, LevelRankInfo value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }
    }
    
    public class AvailableRound {
        public bool found { get; set; }
        public List<RankRound> leaderboards { get; set; }
    }

    public class RankRound {
        public string queryname { get; set; }
        public string[] ids { get; set; }
    }
    
    public class RoundConverter : JsonConverter<RankRound> {
        public override RankRound Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            RankRound rankRound = new RankRound();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    return rankRound;
                }

                if (reader.TokenType == JsonTokenType.PropertyName) {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName) {
                        case "_name":
                            rankRound.queryname = reader.GetString();
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
                                                rankRound.ids = new[] { reader.GetString() };
                                                break;
                                            case JsonTokenType.StartArray:
                                                rankRound.ids = JsonSerializer.Deserialize<string[]>(ref reader);
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

        public override void Write(Utf8JsonWriter writer, RankRound value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }
    }
    
    public class SearchResult {
        public bool found { get; set; }
        public List<SearchPlayer> users { get; set; }
    }
    
    public class SearchPlayer {
        public string onlineServiceType { get; set; }
        public string onlineServiceId { get; set; }
        public string onlineServiceNickname { get; set; }
        public bool isAnonymous { get; set; }
        public string country { get; set; }
        public string id { get; set; }
    }
    
    public class PlayerStats {
        public bool found { get; set; }
        public User user { get; set; }
        public List<PbInfo> pbs { get; set; }
        public OverallInfo speedrunrank { get; set; }
    }

    public class User {
        public string onlineServiceType { get; set; }
        public string onlineServiceId { get; set; }
        public string onlineServiceNickname { get; set; }
        public bool isAnonymous { get; set; }
        public string country { get; set; }
    }
    
    public class PbInfo {
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
    
    public class OverallInfo {
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
}