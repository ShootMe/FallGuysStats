using System;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace FallGuysStats {
    public class ZipWebClient : WebClient {
        public ZipWebClient() : base() {
            this.Encoding = Encoding.GetEncoding(1252);
            this.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0)";
            this.Headers["Accept-Encoding"] = "gzip, deflate";
            this.Headers["Accept-Language"] = "en-us";
            this.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
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