using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FallGuysStats {
    internal class FallalyticsReporter {
        private static readonly string ReportAPIEndpoint = "https://fallalytics.com/api/report";
        public static readonly string RegisterPbAPIEndpoint = "https://fallalytics.com/api/best-time";
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task RegisterPb(RoundInfo stat, double record, DateTime finish, bool isAnonymous, string apiKey) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, RegisterPbAPIEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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
            request.Content = new StringContent(this.RoundInfoToReportJsonString(stat), Encoding.UTF8, "application/json");
            try {
                await HttpClient.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine($@"Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: {e.Message}");
            }
        }
        
        private string RoundInfoToReportJsonString(RoundInfo round) {
            var payload = new {
                round = round.Name,
                index = round.Round,
                show = round.ShowNameId,
                isfinal = round.IsFinal,
                session = round.SessionId
            };
            return JsonSerializer.Serialize(payload);
        }
        
        private string RoundInfoToRegisterPbJsonString(RoundInfo round, double record, DateTime finish, bool isAnonymous) {
            var payload = new {
                country = Stats.HostCountryCode,
                onlineServiceType = $"{(int)Stats.OnlineServiceType}",
                onlineServiceId = Stats.OnlineServiceId,
                onlineServiceNickname = Stats.OnlineServiceNickname,
                isAnonymous,
                finish = $"{finish:o}",
                record,
                round = round.Name,
                show = round.ShowNameId,
                session = round.SessionId
            };
            return JsonSerializer.Serialize(payload);
        }
    }
}
