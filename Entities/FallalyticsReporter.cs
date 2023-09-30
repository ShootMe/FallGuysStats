using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FallGuysStats {
    internal class FallalyticsReporter {
        private List<RoundInfo> roundList = new List<RoundInfo>();

        private static readonly string ReportAPIEndpoint = "https://fallalytics.com/api/report";
        
        public static readonly string RegisterPbAPIEndpoint = "https://fallalytics.com/api/best-time";

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task RegisterPb(RoundInfo stat, double record, string apiKey, bool isAnonymous) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RegisterPbAPIEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(this.RoundInfoToRegisterPbJsonString(stat, record, isAnonymous), Encoding.UTF8, "application/json");
            try {
                await HttpClient.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }
        }
        
        public async Task Report(RoundInfo stat, string apiKey) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ReportAPIEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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
                await this.ShowComplete(apiKey);
            }
        }
        private async Task ShowComplete(string apiKey) {
            HttpRequestMessage requestArray = new HttpRequestMessage(HttpMethod.Post, ReportAPIEndpoint);
            requestArray.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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
            strBuilder.Append($"\"index\":\"{round.Round}\",");
            strBuilder.Append($"\"show\":\"{round.ShowNameId}\",");
            strBuilder.Append($"\"isfinal\":\"{round.IsFinal.ToString().ToLower()}\",");
            strBuilder.Append($"\"session\":\"{round.SessionId}\"}}");
            return strBuilder.ToString();
        }
        private string RoundInfoToRegisterPbJsonString(RoundInfo round, double record, bool isAnonymous) {
            StringBuilder strBuilder = new StringBuilder();
            string token = string.Empty;
            string[] data = new string[9];
            data[0] = $"{round.Name}";
            data[1] = $"{round.ShowNameId}";
            data[2] = $"{record}";
            data[3] = $"{round.Finish.Value:o}";
            data[4] = $"{Stats.HostCountry}";
            data[5] = $"{(int)Stats.OnlineServiceType}";
            data[6] = $"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceId)}";
            data[7] = $"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceNickname)}";
            data[8] = $"{round.SessionId}";
            
            int[] ra = new int[9];
            for (int i = 0; i < ra.Length; i++) {
                ra[i] = i;
            }
            Random random = new Random();
            for (int i = ra.Length - 1; i > 0; i--) {
                int j = random.Next(i + 1);
                (ra[i], ra[j]) = (ra[j], ra[i]);
            }
            
            for (int i = 0; i < ra.Length; i++) {
                token += data[i];
            }
            token = Stats.ComputeHash(Encoding.UTF8.GetBytes(token), Stats.HashTypes.SHA256);
            token += string.Join("", ra);
            
            strBuilder.Append($"{{\"round\":\"{data[0]}\",");
            strBuilder.Append($"\"show\":\"{data[1]}\",");
            strBuilder.Append($"\"record\":\"{data[2]}\",");
            strBuilder.Append($"\"finishDate\":\"{data[3]}\",");
            strBuilder.Append($"\"userCountry\":\"{data[4]}\",");
            strBuilder.Append($"\"onlineServiceType\":\"{data[5]}\",");
            strBuilder.Append($"\"onlineServiceId\":\"{data[6]}\",");
            strBuilder.Append($"\"onlineServiceNickname\":\"{data[7]}\",");
            strBuilder.Append($"\"session\":\"{data[8]}\",");
            strBuilder.Append($"\"token\":\"{token}\"}}");
            return strBuilder.ToString();
        }
    }
}
