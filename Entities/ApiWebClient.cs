using System;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace FallGuysStats {
    public class ApiWebClient : WebClient {
        public bool UseWebProxy = true;
        public ApiWebClient() {
            this.Encoding = Encoding.UTF8;
            this.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64;)";
            this.Headers["Accept-Encoding"] = "gzip, deflate";
            this.Headers["Accept-Language"] = "en-US";
            this.Headers["Accept"] = "application/json";
        }
        
        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (this.UseWebProxy && Stats.UseWebProxy && Stats.SucceededTestProxy) {
                WebProxy webproxy = new WebProxy($"{Stats.ProxyAddress}:{(!string.IsNullOrEmpty(Stats.ProxyPort) ? Stats.ProxyPort : "80")}", false) {
                    BypassProxyOnLocal = false
                };

                if (Stats.EnableProxyAuthentication && !string.IsNullOrEmpty(Stats.ProxyUsername) && !string.IsNullOrEmpty(Stats.ProxyPassword)) {
                    webproxy.Credentials = new NetworkCredential(Stats.ProxyUsername, Stats.ProxyPassword);
                }
                
                request.Proxy = webproxy;
            }
            
            return request;
        }
    }
}