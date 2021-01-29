using System.Text.Json.Serialization;

namespace Microsoft.BotBuilderSamples
{
    public class LineChannel
    {
        public object Message { get; set; }
        public string ReplyToken { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("originUrl")]
        public string originUrl { get; set; }
        [JsonPropertyName("previewUrl")]
        public string previewUrl { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
    }

    public class Text
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string TextString { get; set; }
    }

}