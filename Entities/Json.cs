using System;
using System.Collections.Generic;
using System.Text;
namespace FallGuysStats {
    public static class Json {
        public static JsonObject Read(string json) {
            for (int i = 0; i < json.Length; i++) {
                char c = json[i];
                if (c == '[') {
                    return new JsonArray(null, json, ref i);
                } else if (c == '{') {
                    return new JsonClass(null, json, ref i);
                }
            }
            return null;
        }
        public static JsonObject Read(byte[] json) {
            return Read(Encoding.UTF8.GetString(json));
        }
    }
    public class JsonArray : JsonObject {
        private List<JsonObject> Objects { get; set; }
        public int Count { get { return Objects.Count; } }
        public JsonArray(JsonObject parent = null) {
            Parent = parent;
            Objects = new List<JsonObject>();
        }
        internal JsonArray(JsonObject parent, string json, ref int index) {
            Parent = parent;
            Objects = new List<JsonObject>();
            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if (char.IsWhiteSpace(c)) { continue; }

                if (c == '[') {
                    Objects.Add(new JsonArray(this, json, ref i));
                } else if (c == '{') {
                    Objects.Add(new JsonClass(this, json, ref i));
                } else if (c == ']') {
                    index = i;
                    break;
                } else if (c != ',') {
                    Objects.Add(new JsonItem(this, json, false, ref i));
                }
            }
        }
        public JsonObject this[int index] {
            get { return Objects[index]; }
            set { Objects[index] = value; }
        }
        public void Add(object value, bool isString = false) {
            if (value is JsonObject jObj) {
                jObj.Parent = this;
                Objects.Add(jObj);
            } else {
                Objects.Add(new JsonItem(value == null ? null : $"{value}", isString, this));
            }
        }
        public override string Value() { return ToString(); }
        public override string ToString() { return ToString(0); }
        internal override string ToString(int level) {
            string padding = string.Empty.PadLeft(level * 2);
            StringBuilder values = new StringBuilder();
            values.AppendLine("[");
            for (int i = 0; i < Objects.Count; i++) {
                values.Append(padding).Append("  ").Append($"{Objects[i].ToString(level + 1)}, ");
                if (i + 1 < Objects.Count) {
                    values.AppendLine();
                }
            }
            if (values.Length > 1) {
                values.Length -= 2;
            }
            values.AppendLine().Append(padding).Append(']');
            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            for (int i = 0; i < Objects.Count; i++) {
                IEnumerator<JsonObject> e = Objects[i].Enumerate();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }
    }
    public class JsonClass : JsonObject {
        private Dictionary<string, JsonObject> Fields { get; set; }

        public JsonClass(JsonObject parent = null) {
            Parent = parent;
            Fields = new Dictionary<string, JsonObject>();
        }
        internal JsonClass(JsonObject parent, string json, ref int index) {
            Parent = parent;
            Fields = new Dictionary<string, JsonObject>();

            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if (c == '"') {
                    string field = ReadField(json, ref i);
                    bool foundColon = false;
                    do {
                        c = json[++i];
                        if (c == ':') {
                            if (foundColon) { break; }
                            foundColon = true;
                        }
                    } while (char.IsWhiteSpace(c) || c == ':');

                    if (c == '{') {
                        AddField(field, new JsonClass(this, json, ref i));
                    } else if (c == '[') {
                        AddField(field, new JsonArray(this, json, ref i));
                    } else {
                        AddField(field, new JsonItem(this, json, c == '"', ref i));
                    }
                } else if (c == '}') {
                    index = i;
                    break;
                }
            }
        }
        private void AddField(string field, JsonObject value) {
            if (!Fields.ContainsKey(field)) {
                Fields[field] = value;
            }
        }
        public void Add(string field, object value, bool isString = false) {
            if (!Fields.ContainsKey(field)) {
                if (value is JsonObject jObj) {
                    jObj.Parent = this;
                    Fields[field] = jObj;
                } else {
                    Fields[field] = new JsonItem(value == null ? null : $"{value}", isString, this);
                }
            }
        }
        private string ReadField(string json, ref int index) {
            int start = index;
            char c = json[index], last;
            do {
                last = c;
                c = json[++index];
            } while (c != '"' || last == '\\');
            return json.Substring(start + 1, index - start - 1);
        }
        public JsonObject this[string key] {
            get {
                return Fields.TryGetValue(key, out JsonObject value) ? value : JsonObject.EMPTY;
            }
        }
        public override string Value() { return ToString(); }
        public override string ToString() { return ToString(0); }
        internal override string ToString(int level) {
            string padding = string.Empty.PadLeft(level * 2);
            StringBuilder values = new StringBuilder();
            if (Fields.Count > 0) {
                values.AppendLine("{");
                int count = 0;
                foreach (KeyValuePair<string, JsonObject> pair in Fields) {
                    count++;
                    values.Append(padding).Append("  ").Append($"\"{pair.Key}\": {pair.Value.ToString(level + 1)}, ");
                    if (count < Fields.Count) {
                        values.AppendLine();
                    }
                }

                if (values.Length > 1) {
                    values.Length -= 2;
                }
                values.AppendLine().Append(padding).Append('}');
            } else {
                values.Append("{}");
            }
            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            return Fields.Values.GetEnumerator();
        }
    }
    public class JsonItem : JsonObject {
        public string Item { get; set; }
        public bool IsString { get; set; }

        public JsonItem(string value, bool isString = false, JsonObject parent = null) {
            Parent = parent;
            Item = value;
            IsString = isString;
        }
        internal JsonItem(JsonObject parent, string json, bool requireEndQuote, ref int index) {
            IsString = requireEndQuote;
            Parent = parent;
            char last = '\0';
            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if ((requireEndQuote && c == '"' && last != '\\') || (!requireEndQuote && (c == ']' || c == '}' || (c == '"' && last != '\\') || c == ','))) {
                    Item = c == '"' ? json.Substring(index + 1, i - index - 1) : json.Substring(index, i - index).Trim();
                    if (c == '"') {
                        do {
                            c = json[++i];
                        } while (c != ']' && c != '}' && c != ',' && c != '"');
                    }
                    index = c == ']' || c == '}' || c == '"' ? i - 1 : i;
                    break;
                }
                last = c;
            }
        }
        public override string Value() { return Item; }
        public override string ToString() { return Item == null ? "null" : IsString ? $"\"{Item}\"" : Item; }
        internal override string ToString(int level) { return ToString(); }
    }
    public abstract class JsonObject : IEnumerable<JsonObject> {
        public static JsonObject EMPTY = new JsonItem("null");
        public JsonObject Parent { get; set; }
        public virtual string Value() { return string.Empty; }
        internal virtual string ToString(int level) { return string.Empty; }
        public override string ToString() { return string.Empty; }
        internal virtual IEnumerator<JsonObject> Enumerate() { yield return this; }
        public IEnumerator<JsonObject> GetEnumerator() { return Enumerate(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public string AsString() {
            string value = Value();
            return value == "null" ? null : value;
        }
        public bool AsBool() {
            string value = AsString();
            return !string.IsNullOrEmpty(value) && bool.TryParse(value, out bool result) && result;
        }
        public long AsLong() {
            string value = AsString();
            return string.IsNullOrEmpty(value) || !long.TryParse(value, out long result) ? 0 : result;
        }
        public int AsInt() {
            string value = AsString();
            return string.IsNullOrEmpty(value) || !int.TryParse(value, out int result) ? 0 : result;
        }
        public decimal AsDecimal() {
            string value = AsString();
            return string.IsNullOrEmpty(value) || !decimal.TryParse(value, out decimal result) ? 0 : result;
        }
        public DateTime AsDate() {
            string value = AsString();
            return string.IsNullOrEmpty(value) || !DateTime.TryParse(value, out DateTime result) ? DateTime.MinValue : result;
        }
    }
}