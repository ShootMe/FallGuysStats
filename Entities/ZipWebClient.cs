using System;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace FallGuysStats {
    public class ZipWebClient : WebClient {
        public ZipWebClient() {
            this.Encoding = Encoding.UTF8;
            this.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64;)";
            this.Headers["Accept-Encoding"] = "gzip, deflate";
            this.Headers["Accept-Language"] = "en-US";
            this.Headers["Accept"] = "application/zip";
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}