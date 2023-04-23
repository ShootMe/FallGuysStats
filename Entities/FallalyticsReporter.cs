using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace FallGuysStats.Entities {
    internal class FallalyticsReporter {

        public List<RoundInfo> gameList = new List<RoundInfo>();

        private static readonly string APIEndpoint = "https://fallalytics.com/api/report";

        private static readonly HttpClient client = new HttpClient();

        public async void Report(RoundInfo stat, string APIKey) {
            var request = new HttpRequestMessage(HttpMethod.Post, APIEndpoint);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);

            request.Content = new StringContent(RoundInfoToJSONString(stat), Encoding.UTF8, "application/json");
            try {
                await client.SendAsync(request);
            } catch (HttpRequestException e) {
                Console.WriteLine("Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: " + e.ToString());
            }

            if(stat.Round == 1) {
                foreach (RoundInfo game in gameList.ToList()) {
                    if(game.ShowID != stat.ShowID) {
                        gameList.Remove(game);
                    }
                } 
            }

            gameList.Add(stat);

            bool foundMissMatch = false;
            int finalRound = -1;
            foreach (RoundInfo game in gameList.ToList()) {
                if(game.SessionId != stat.SessionId) {
                    foundMissMatch = true;
                }
                if (game.IsFinal) {
                    finalRound = game.Round;
                }
            }
            if (gameList.Count == finalRound && !foundMissMatch) {
                gameComplete(APIKey);
            }
        }
        public async void gameComplete(string APIKey) {
            var requestArray = new HttpRequestMessage(HttpMethod.Post, APIEndpoint);
            requestArray.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
            string jsonArraystring = "[";
            foreach (RoundInfo game in gameList) {
                jsonArraystring += RoundInfoToJSONString(game);
                jsonArraystring += ",";
            }
            jsonArraystring = jsonArraystring.Remove(jsonArraystring.Length - 1);
            jsonArraystring += "]";

            requestArray.Content = new StringContent(jsonArraystring, Encoding.UTF8, "application/json");

            try {
                await client.SendAsync(requestArray);
            } catch (HttpRequestException e) {
                Console.WriteLine("Error in FallalyticsReporter. Should not be a problem as it only affects the reporting. Error: " + e.ToString());
            }
            
            gameList = new List<RoundInfo>();
        }
        public string RoundInfoToJSONString(RoundInfo round) {
            string JSON = "";
            JSON += "{\"round\":\"" + round.Name + "\",";
            JSON += "\"index\":" + round.Round + ",";
            JSON += "\"show\":\"" + round.ShowNameId + "\",";
            JSON += "\"isfinal\":" + round.IsFinal.ToString().ToLower() + ",";
            JSON += "\"session\":\"" + round.SessionId + "\"}";
            return JSON;
        }
    }
}
