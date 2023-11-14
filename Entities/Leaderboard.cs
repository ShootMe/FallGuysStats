using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FallGuysStats {
    public class Leaderboard {
        public bool found { get; set; }
        public int total { get; set; }
        public List<RankInfo> recordholders { get; set; }
    }
    
    public class RankInfo {
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

    public class RecordHolderConverter : JsonConverter<RankInfo> {
        public override RankInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var recordHolder = new RankInfo();
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
                                // recordHolder.onlineServiceType = Multilingual.GetWord("leaderboard_grid_anonymous");
                                recordHolder.onlineServiceId = reader.GetString();
                                recordHolder.onlineServiceNickname = $"👻 {Multilingual.GetWord("leaderboard_grid_anonymous")}";
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

        public override void Write(Utf8JsonWriter writer, RankInfo value, JsonSerializerOptions options) {
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
}