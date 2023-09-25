using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace FallGuysStats {
    internal class FallalyticsReporter {
        private List<RoundInfo> roundList = new List<RoundInfo>();

        private static readonly string ReportAPIEndpoint = "https://fallalytics.com/api/report";
        
        private static readonly string RegisterPbAPIEndpoint = "https://fallalytics.com/api/best-time";

        private static readonly HttpClient HttpClient = new HttpClient();

        public async void RegisterPb(RoundInfo stat, double record, string APIKey, bool isAnonymous) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RegisterPbAPIEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
            request.Content = new StringContent(this.RoundInfoToRegisterPbJsonString(stat, record, isAnonymous), Encoding.UTF8, "application/json");
            try {
                await HttpClient.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }
        }
        
        public async void Report(RoundInfo stat, string APIKey) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ReportAPIEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
            request.Content = new StringContent(this.RoundInfoToReportJsonString(stat), Encoding.UTF8, "application/json");
            try {
                await HttpClient.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }

            if(stat.Round == 1) {
                foreach (RoundInfo roundInfo in this.roundList.ToList()) {
                    if(roundInfo.ShowID != stat.ShowID) {
                        this.roundList.Remove(roundInfo);
                    }
                } 
            }

            this.roundList.Add(stat);

            bool foundMissMatch = false;
            int finalRound = -1;
            foreach (RoundInfo roundInfo in this.roundList.ToList()) {
                if(roundInfo.SessionId != stat.SessionId) {
                    foundMissMatch = true;
                }
                if (roundInfo.IsFinal) {
                    finalRound = roundInfo.Round;
                }
            }
            if (this.roundList.Count == finalRound && !foundMissMatch) {
                this.ShowComplete(APIKey);
            }
        }
        private async void ShowComplete(string APIKey) {
            HttpRequestMessage requestArray = new HttpRequestMessage(HttpMethod.Post, ReportAPIEndpoint);
            requestArray.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
            string jsonArraystring = "[";
            foreach (RoundInfo game in this.roundList) {
                jsonArraystring += this.RoundInfoToReportJsonString(game);
                jsonArraystring += ",";
            }
            jsonArraystring = jsonArraystring.Remove(jsonArraystring.Length - 1);
            jsonArraystring += "]";

            requestArray.Content = new StringContent(jsonArraystring, Encoding.UTF8, "application/json");

            try {
                await HttpClient.SendAsync(requestArray);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }
            
            this.roundList = new List<RoundInfo>();
        }
        private string RoundInfoToReportJsonString(RoundInfo round) {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"{{\"round\":\"{round.Name}\",");
            strBuilder.Append($"\"index\":{round.Round}\",");
            strBuilder.Append($"\"show\":\"{round.ShowNameId}\",");
            strBuilder.Append($"\"isfinal\":{round.IsFinal.ToString().ToLower()}\",");
            strBuilder.Append($"\"session\":\"{round.SessionId}\"}}");
            return strBuilder.ToString();
        }
        private string RoundInfoToRegisterPbJsonString(RoundInfo round, double record, bool isAnonymous) {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"{{\"round\":\"{round.Name}\",");
            strBuilder.Append($"\"show\":\"{round.ShowNameId}\",");
            strBuilder.Append($"\"record\":\"{record}\",");
            strBuilder.Append($"\"finishDate\":\"{round.Finish.Value:o}\",");
            strBuilder.Append($"\"userCountry\":\"{Stats.HostCountry}\",");
            strBuilder.Append($"\"onlineServiceType\":\"{(int)Stats.OnlineServiceType}\",");
            strBuilder.Append($"\"onlineServiceId\":\"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceId)}\",");
            strBuilder.Append($"\"onlineServiceNickname\":\"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceNickname)}\",");
            strBuilder.Append($"\"session\":\"{round.SessionId}\"}}");
            return strBuilder.ToString();
        }
    }
}
