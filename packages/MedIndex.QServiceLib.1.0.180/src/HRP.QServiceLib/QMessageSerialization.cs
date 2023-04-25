using System.Net.Mime;

namespace HRP.QServiceLib
{
    public static class QMessageSerialization
    {
        public static ContentType binaryContentType = new ContentType() { Name = "application/octet-stream", MediaType = "application/octet-stream", CharSet = "base64" };
        public static ContentType jsonContentType = new ContentType() { Name = "application/json", MediaType = "application/json", CharSet = "utf-8" };

        public static bool IsJsonMessage(string mimeContentType)
        {
            return !string.IsNullOrWhiteSpace(mimeContentType) && mimeContentType.Trim() == "application/json";
        }
    }
}
