using System.IO;
using System.Text;
namespace FallGuysStats {
    public class LineReader {
        private readonly byte[] buffer;
        private int bufferIndex, bufferSize;
        private readonly Stream file;
        private readonly StringBuilder currentLine;
        public long Position;
        public LineReader(Stream stream) {
            file = stream;
            buffer = new byte[1024];
            currentLine = new StringBuilder();
            Position = stream.Position;
        }

        public string ReadLine() {
            while (bufferIndex < bufferSize) {
                byte data = buffer[bufferIndex++];
                Position++;

                if (data == (byte)'\n' || data == (byte)'\r') {
                    if (data == '\r') {
                        data = bufferIndex < buffer.Length ? buffer[bufferIndex] : (byte)0;
                        if (data == (byte)'\n') {
                            bufferIndex++;
                            Position++;
                        }
                    }

                    string result = currentLine.ToString();
                    currentLine.Clear();
                    return result;
                }

                currentLine.Append((char)data);
            }

            while ((bufferSize = file.Read(buffer, 0, buffer.Length)) > 0) {
                bufferIndex = 0;
                while (bufferIndex < bufferSize) {
                    byte data = buffer[bufferIndex++];
                    Position++;

                    if (data == (byte)'\n' || data == (byte)'\r') {
                        if (data == '\r') {
                            data = bufferIndex < buffer.Length ? buffer[bufferIndex] : (byte)0;
                            if (data == (byte)'\n') {
                                bufferIndex++;
                                Position++;
                            }
                        }

                        string result = currentLine.ToString();
                        currentLine.Clear();
                        return result;
                    }

                    currentLine.Append((char)data);
                }
            }

            if (currentLine.Length > 0) {
                string result = currentLine.ToString();
                currentLine.Clear();
                return result;
            }
            return null;
        }
    }
}