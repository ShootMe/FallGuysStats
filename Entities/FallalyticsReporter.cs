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

        public async Task RegisterPb(RoundInfo stat, double record, DateTime finish, bool isAnonymous) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RegisterPbAPIEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Utils.ComputeHash(Encoding.UTF8.GetBytes($"{finish:o}{Environment.GetEnvironmentVariable("FALLALYTICS_KEY")}"), HashTypes.SHA256));
            request.Headers.Referrer = new Uri($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.eunma.io");
            request.Content = new StringContent(this.RoundInfoToRegisterPbJsonString(stat, record, finish, isAnonymous), Encoding.UTF8, "application/json");
            try {
                await HttpClient.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }
        }
        
        public async Task Report(RoundInfo stat, string apiKey) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ReportAPIEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Headers.Referrer= new Uri($"FallGuysStatsV{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}");
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
        private string RoundInfoToRegisterPbJsonString(RoundInfo round, double record, DateTime finish, bool isAnonymous) {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"{{\"country\":\"{Stats.HostCountryCode}\",");
            strBuilder.Append($"\"onlineServiceType\":\"{(int)Stats.OnlineServiceType}\",");
            strBuilder.Append($"\"onlineServiceId\":\"{Stats.OnlineServiceId}\",");
            strBuilder.Append($"\"onlineServiceNickname\":\"{Stats.OnlineServiceNickname}\",");
            strBuilder.Append($"\"isAnonymous\":\"{isAnonymous}\",");
            strBuilder.Append($"\"finish\":\"{finish:o}\",");
            strBuilder.Append($"\"record\":\"{record}\",");
            strBuilder.Append($"\"round\":\"{round.Name}\",");
            strBuilder.Append($"\"show\":\"{round.ShowNameId}\",");
            strBuilder.Append($"\"session\":\"{round.SessionId}\"}}");
            return strBuilder.ToString();
        }
    }
}
