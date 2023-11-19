using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FallGuysStats {
	public static class Utils {
        public static readonly string GITHUB_API_URL = "https://api.github.com/";
        public static readonly string FALLGUYSSTATS_RELEASES_LATEST_DOWNLOAD_URL = "https://github.com/ShootMe/FallGuysStats/releases/latest/download/FallGuysStats.zip";
        public static readonly string FALLGUYSDB_API_URL = "https://api2.fallguysdb.info/api/";
        private static readonly string IP2C_ORG_URL = "https://ip2c.org/"; // https://ip2c.org/{ip}
        private static readonly string IPINFO_IO_URL = "https://ipinfo.io/"; // https://ipinfo.io/{ip}/json or https://ipinfo.io/ip
        private static readonly string IPAPI_COM_URL = "http://ip-api.com/json/"; // http://ip-api.com/json/{ip}
        private static readonly string NORDVPN_COM_URL = "https://nordvpn.com/wp-admin/admin-ajax.php?action=get_user_info_data&ip="; // https://nordvpn.com/wp-admin/admin-ajax.php?action=get_user_info_data&ip={ip}
        
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("User32.dll")]
        public static extern bool MoveWindow(IntPtr h, int x, int y, int width, int height, bool redraw);
        
        //[DllImport("user32.dll")]
        //private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetForegroundWindow();
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetActiveWindow();
        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern long DwmSetWindowAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hwnd, int time, int flags);

        public static string GetGuid() {
			var guid = Guid.NewGuid();
			return guid.ToString("N");
		}
        
        public static bool IsProcessRunning(string processName) {
            try {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0) {
                    return true;
                }
                return false;
            } catch {
                return false;
            }
        }
        
        public static Bitmap ImageOpacity(Image sourceImage, float opacity = 1F) {
            Bitmap bmp = new Bitmap(sourceImage.Width, sourceImage.Height);
            Graphics gp = Graphics.FromImage(bmp);
            ColorMatrix clrMatrix = new ColorMatrix { Matrix33 = opacity };
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gp.DrawImage(sourceImage, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, imgAttribute);
            gp.Dispose();
            return bmp;
        }
        
        public static Bitmap ResizeImageWidth(Image source, int width) {
            float ratio = (float)width / source.Width;
            return new Bitmap(source, new Size((int)(source.Width * ratio), (int)(source.Height * ratio)));
        }
        
        public static Bitmap ResizeImageHeight(Image source, int height) {
            float ratio = (float)height / source.Height;
            return new Bitmap(source, new Size((int)(source.Width * ratio), (int)(source.Height * ratio)));
        }
        
        public static Bitmap ResizeImageScale(Image source, float scale) {
            if (scale <= 0) { scale = 1; }
            return new Bitmap(source, new Size((int)(source.Width * scale), (int)(source.Height * scale)));
        }
        
        public static Color GetComplementaryColor(Color source, int alpha = 255) {
            return Color.FromArgb(Math.Min(255, Math.Max(0, alpha)), Math.Min(255, Math.Max(0, 255 - source.R)), Math.Min(255, Math.Max(0, 255 - source.G)), Math.Min(255, Math.Max(0, 255 - source.B)));
        }
        
        public static Color GetColorBrightnessAdjustment(Color sourceColor, float fBrightness) {
            return Color.FromArgb(Math.Min(255, Math.Max(0, (int)(sourceColor.R * fBrightness))), Math.Min(255, Math.Max(0, (int)(sourceColor.G * fBrightness))), Math.Min(255, Math.Max(0, (int)(sourceColor.B * fBrightness))));
        }
        
        public static string ComputeHash(byte[] input, HashTypes hashType) {
            if (input == null || input.Length == 0) return string.Empty;
            HashAlgorithm hashAlgorithm = null;
            switch (hashType) {
                case HashTypes.MD5:
                    hashAlgorithm = MD5.Create(); break;
                case HashTypes.RIPEMD160:
                    hashAlgorithm = RIPEMD160.Create(); break;
                case HashTypes.SHA1:
                    hashAlgorithm = SHA1.Create(); break;
                case HashTypes.SHA256:
                    hashAlgorithm = SHA256.Create(); break;
                case HashTypes.SHA384:
                    hashAlgorithm = SHA384.Create(); break;
                case HashTypes.SHA512:
                    hashAlgorithm = SHA512.Create(); break;
            }

            if (hashAlgorithm != null) {
                return BitConverter.ToString(hashAlgorithm.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
            }

            return string.Empty;
        }
        
        // public T[] ConcatArrays<T>(params T[][] sourceArrays) {
        //     int totalLength = sourceArrays.Sum(arr => arr.Length);
        //     T[] destinationArray = new T[totalLength];
        //
        //     int destinationIndex = 0;
        //     foreach (T[] sourceArray in sourceArrays) {
        //         Array.Copy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
        //         destinationIndex += sourceArray.Length;
        //     }
        //
        //     return destinationArray;
        // }
        
        public static bool IsOnScreen(int x, int y, int w, int h) {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens) {
                if (screen.WorkingArea.Contains(new Point(x, y)) || screen.WorkingArea.Contains(new Point(x + w, y + h))) {
                    return true;
                }
            }
            return false;
        }
        
        public static Screen GetCurrentScreen(Point location) {
            Screen[] scr = Screen.AllScreens;
            Screen screen = Screen.PrimaryScreen;
            foreach (Screen s in scr) {
                if (s.WorkingArea.Contains(location)) {
                    screen = s;
                    break;
                }
            }
            return screen;
        }
        
        public static bool IsInternetConnected() {
            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try {
                // Check NCSI test link
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT){
                    return false;
                }

                // Check NCSI DNS IP
                IPHostEntry dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() < 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS) {
                    return false;
                }
            } catch {
                return false;
            }

            return true;
        }
        
        public static bool IsDomainNameValid(string url) {
            try {
                Uri uri = new Uri(url);
                string host = uri.Host;
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                return addresses.Length > 0;
            } catch {
                return false;
            }
        }
        
        public static bool IsEndpointValid(string url) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode == HttpStatusCode.OK) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch {
                return false;
            }
        }
        
        private static string[] GetCountryCodeUsingIp2c(string host) {
            string[] countryInfo = { string.Empty, string.Empty };
            using (ApiWebClient web = new ApiWebClient()) {
                string resStr = Regex.Unescape(web.DownloadString($"{IP2C_ORG_URL}{host}"));
                string[] resArr = resStr.Split(';');
                if ("1".Equals(resArr[0])) {
                    countryInfo[0] = resArr[1]; // alpha-2 code
                    countryInfo[1] = resArr[3]; // a full country name
                }
            }
            return countryInfo;
        }
        
        private static string[] GetCountryCodeUsingIpinfo(string host) {
            string[] countryInfo = { string.Empty, string.Empty, string.Empty };
            using (ApiWebClient web = new ApiWebClient()) {
                string resJsonStr = web.DownloadString($"{IPINFO_IO_URL}{host}/json");
                JsonClass json = Json.Read(resJsonStr) as JsonClass;
                if (!string.IsNullOrEmpty(json["country"].AsString())) countryInfo[0] = Regex.Unescape(json["country"].AsString()); // alpha-2 code
                if (!string.IsNullOrEmpty(json["region"].AsString())) countryInfo[1] = Regex.Unescape(json["region"].AsString());
                if (!string.IsNullOrEmpty(json["city"].AsString())) countryInfo[2] = Regex.Unescape(json["city"].AsString());
            }
            return countryInfo;
        }
        
        private static string[] GetCountryCodeUsingIpapi(string host) {
            string[] countryInfo = { string.Empty, string.Empty, string.Empty };
            using (ApiWebClient web = new ApiWebClient()) {
                string resJsonStr = web.DownloadString($"{IPAPI_COM_URL}{host}");
                JsonClass json = Json.Read(resJsonStr) as JsonClass;
                if (!string.IsNullOrEmpty(json["countryCode"].AsString())) countryInfo[0] = Regex.Unescape(json["countryCode"].AsString()); // alpha-2 code
                // if (!string.IsNullOrEmpty(json["country"].AsString())) countryInfo[1] = Regex.Unescape(json["country"].AsString()); // a full country name
                if (!string.IsNullOrEmpty(json["regionName"].AsString())) countryInfo[1] = Regex.Unescape(json["regionName"].AsString());
                if (!string.IsNullOrEmpty(json["city"].AsString())) countryInfo[2] = Regex.Unescape(json["city"].AsString());
            }
            return countryInfo;
        }
        
        public static string[] GetCountryCodeUsingNordvpn(string host) {
            string[] countryInfo = { string.Empty, string.Empty, string.Empty };
            using (ApiWebClient web = new ApiWebClient()) {
                string resJsonStr = web.DownloadString($"{NORDVPN_COM_URL}{host}");
                JsonClass json = Json.Read(resJsonStr) as JsonClass;
                if (!string.IsNullOrEmpty(json["country_code"].AsString())) countryInfo[0] = Regex.Unescape(json["country_code"].AsString()); // alpha-2 code
                // if (!string.IsNullOrEmpty(json["country"].AsString())) countryInfo[1] = Regex.Unescape(json["country"].AsString()); // a full country name
                if (!string.IsNullOrEmpty(json["region"].AsString())) countryInfo[1] = Regex.Unescape(json["region"].AsString());
                if (!string.IsNullOrEmpty(json["city"].AsString())) countryInfo[2] = Regex.Unescape(json["city"].AsString());
            }
            return countryInfo;
        }
        
        public static JsonElement GetApiData(string apiUrl, string apiEndPoint) {
            JsonElement resJroot;
            using (ApiWebClient web = new ApiWebClient()) {
                string responseJsonString = web.DownloadString($"{apiUrl}{apiEndPoint}");
                JsonDocument jdom = JsonDocument.Parse(responseJsonString);
                resJroot = jdom.RootElement;
            }
            return resJroot;
        }
        
        public static string GetUserPublicIp() {
            using (ApiWebClient web = new ApiWebClient()) {
                string publicIp;
                try {
                    publicIp = web.DownloadString($"{IPINFO_IO_URL}ip").Trim();
                } catch {
                    publicIp = string.Empty;
                }
                return publicIp;
            }
        }

        public static string GetCountryCode(string ip) {
            if (string.IsNullOrEmpty(ip)) { return string.Empty; }
            string countryCode = string.Empty;
            try {
                countryCode = GetCountryCodeUsingIp2c(ip)[0]; // alpha-2 code
                if (string.IsNullOrEmpty(countryCode)) {
                    countryCode = GetCountryCodeUsingNordvpn(ip)[0]; // alpha-2 code
                }
                if (string.IsNullOrEmpty(countryCode)) {
                    countryCode = GetCountryCodeUsingIpinfo(ip)[0]; // alpha-2 code
                }
                if (string.IsNullOrEmpty(countryCode)) {
                    countryCode = GetCountryCodeUsingIpapi(ip)[0]; // alpha-2 code
                }
            } catch {
                return string.Empty;
            }
            return countryCode;
        }

        public static string GetCountryInfo(string ip) {
            if (string.IsNullOrEmpty(ip)) { return string.Empty; }
            string countryInfo = string.Empty;
            try {
                string[] rtnValue = GetCountryCodeUsingNordvpn(ip);
                countryInfo = $"{rtnValue[0]};{rtnValue[1]};{rtnValue[2]}"; // alpha-2 code ; region ; city
                if (string.IsNullOrEmpty(rtnValue[0])) {
                    rtnValue = GetCountryCodeUsingIpinfo(ip);
                    countryInfo = $"{rtnValue[0]};{rtnValue[1]};{rtnValue[2]}"; // alpha-2 code ; region ; city
                }
                if (string.IsNullOrEmpty(rtnValue[0])) {
                    rtnValue = GetCountryCodeUsingIpapi(ip);
                    countryInfo = $"{rtnValue[0]};{rtnValue[1]};{rtnValue[2]}"; // alpha-2 code ; region ; city
                }
            } catch {
                return string.Empty;
            }
            return countryInfo;
        }
        
        public static string GetRelativeTime(DateTime targetTime) {
            TimeSpan diff = DateTime.Now - targetTime;
            double days = diff.TotalDays;
            int weeks = (int)Math.Floor(days / 7);
            int months = (int)Math.Floor(days / 30.436875);
            int years = (int)Math.Floor(days / 365.25);
            // if (diff.TotalMinutes < 1)
            //     return Stats.CurrentLanguage == Language.Korean ? "방금 전" : "Just now";
            // else if (diff.TotalMinutes < 60)
            //     return Stats.CurrentLanguage == Language.Korean ? $"{Math.Floor(diff.TotalMinutes)}분 전" : $"{Math.Floor(diff.TotalMinutes)} minutes ago";
            // else if (diff.TotalHours < 24)
            //     return Stats.CurrentLanguage == Language.Korean ? $"{Math.Floor(diff.TotalHours)}시간 전" : $"{Math.Floor(diff.TotalHours)} hour ago";
            if (diff.TotalMinutes <= 10) {
                return Multilingual.GetWord("leaderboard_grid_just_before");
            } else if (days < 1) {
                return Multilingual.GetWord("leaderboard_grid_today");
            } else if (days < 2) {
                return Multilingual.GetWord("leaderboard_grid_yesterday");
            // } else if (days < 3) {
            //     return Multilingual.GetWord("leaderboard_grid_the_day_before_yesterday");
            } else if (days < 7) {
                return $"{Multilingual.GetWord("leaderboard_grid_n_days_ago_prefix")}{Math.Floor(diff.TotalDays)}{Multilingual.GetWord("leaderboard_grid_n_days_ago_suffix")}";
            } else if (weeks < 5) {
                return $"{Multilingual.GetWord("leaderboard_grid_n_weeks_ago_prefix")}{weeks}{Multilingual.GetWord("leaderboard_grid_n_weeks_ago_suffix")}";
            } else if (months < 12) {
                return $"{Multilingual.GetWord("leaderboard_grid_n_months_ago_prefix")}{months}{Multilingual.GetWord("leaderboard_grid_n_months_ago_suffix")}";
            } else {
                return $"{Multilingual.GetWord("leaderboard_grid_n_years_ago_prefix")}{years}{Multilingual.GetWord("leaderboard_grid_n_years_ago_suffix")}";
            }
        }
        
        public static string FormatTime(double value) {
            TimeSpan time = TimeSpan.FromMilliseconds(value);
            if (time.TotalSeconds < 60) {
                return $"{time.TotalSeconds:F3}{Multilingual.GetWord("leaderboard_grid_time_suffix")}";
            } else {
                return $"{time.Minutes}{Multilingual.GetWord("leaderboard_grid_time_prefix")} {time.Seconds:D2}.{time.Milliseconds:D3}{Multilingual.GetWord("leaderboard_grid_time_suffix")}";
            }
        }
    }
}
