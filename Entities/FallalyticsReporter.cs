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
            data[0] = $"{Stats.HostCountry}";
            data[1] = $"{round.Finish.Value:o}";
            data[2] = $"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceId)}";
            data[3] = $"{(isAnonymous ? "Anonymous" : Stats.OnlineServiceNickname)}";
            data[4] = $"{(int)Stats.OnlineServiceType}";
            data[5] = $"{record}";
            data[6] = $"{round.Name}";
            data[7] = $"{round.SessionId}";
            data[8] = $"{round.ShowNameId}";
            
            int[] ra = new int[9];
            for (int i = 0; i < ra.Length; i++) {
                ra[i] = i;
            }
            Random r = new Random();
            for (int i = ra.Length - 1; i > 0; i--) {
                int j = r.Next(i + 1);
                (ra[i], ra[j]) = (ra[j], ra[i]);
            }

            if (1 == r.Next(0, 2)) {
                for (int i = ra.Length - 1; i >= 0; i--) {
                    token += data[i];
                }
                token = Stats.ComputeHash(Encoding.UTF8.GetBytes(token), Stats.HashTypes.SHA256);
                token += Convert.ToBase64String(Encoding.UTF8.GetBytes($"1{string.Join("", ra)}"));
            } else {
                for (int i = 0; i < ra.Length; i++) {
                    token += data[i];
                }
                token = Stats.ComputeHash(Encoding.UTF8.GetBytes(token), Stats.HashTypes.SHA256);
                token += Convert.ToBase64String(Encoding.UTF8.GetBytes($"0{string.Join("", ra)}"));
            }
            
            strBuilder.Append($"{{\"country\":\"{data[0]}\",");
            strBuilder.Append($"\"finishDate\":\"{data[1]}\",");
            strBuilder.Append($"\"onlineServiceId\":\"{data[2]}\",");
            strBuilder.Append($"\"onlineServiceNickname\":\"{data[3]}\",");
            strBuilder.Append($"\"onlineServiceType\":\"{data[4]}\",");
            strBuilder.Append($"\"record\":\"{data[5]}\",");
            strBuilder.Append($"\"round\":\"{data[6]}\",");
            strBuilder.Append($"\"session\":\"{data[7]}\",");
            strBuilder.Append($"\"show\":\"{data[8]}\",");
            strBuilder.Append($"\"token\":\"{token}\"}}");
            return strBuilder.ToString();
        }
    }
}
