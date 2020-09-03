using System;
using System.Text.RegularExpressions;
namespace NewTek.NDI {
    public class Source {
        public string ComputerName { get; private set; }
        public string SourceName { get; private set; }
        public Uri Uri { get; private set; }
        // These are purposely 'public get' only because
        // they should not change during the life of a source.
        private string _name = string.Empty;
        public string Name {
            get { return _name; }
            private set {
                _name = value;

                int parenIdx = _name.IndexOf(" (");
                ComputerName = _name.Substring(0, parenIdx);

                SourceName = Regex.Match(_name, @"(?<=\().+?(?=\))").Value;

                string uriString = string.Format("ndi://{0}/{1}", ComputerName, System.Net.WebUtility.UrlEncode(SourceName));

                Uri uri = null;
                if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri)) {
                    Uri = null;
                } else {
                    Uri = uri;
                }

            }
        }

        // Construct from NDIlib.source_t
        public Source(NDIlib.source_t source_t) {
            Name = UTF.Utf8ToString(source_t.p_ndi_name);
        }

        // Construct from strings
        public Source(string name) {
            Name = name;
        }

        // Copy constructor.
        public Source(Source previousSource) {
            Name = previousSource.Name;
            Uri = previousSource.Uri;
        }
        public override string ToString() {
            return Name;
        }
    }
}