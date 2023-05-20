using System;
using System.Net;
using System.Net.Cache;
using System.Text;
namespace FallGuysStats {
    public class ApiWebClient : WebClient {
        public ApiWebClient() : base() {
            this.Encoding = Encoding.GetEncoding(65001);
            this.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64;)";
            this.Headers["Accept-Encoding"] = "gzip, deflate";
            this.Headers["Accept-Language"] = "en-us";
            this.Headers["Accept"] = "application/json, */*";
        }
        protected override WebRequest GetWebRequest(Uri address) {
            HttpRequestCachePolicy requestPolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromSeconds(10));

            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.CachePolicy = requestPolicy;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}