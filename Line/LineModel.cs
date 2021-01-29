using System.Collections.Generic;
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


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Hero
    {
        public string type { get; set; }
        public string url { get; set; }
        public string align { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public string backgroundColor { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public bool wrap { get; set; }
        public List<object> contents { get; set; }
        public string align { get; set; }
        public string gravity { get; set; }
        public Action action { get; set; }
        public string style { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<Content> contents { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
        public string label { get; set; }
        public string text { get; set; }
    }

    public class Footer
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<Content> contents { get; set; }
    }

    public class Card
    {
        public string type { get; set; }
        public string direction { get; set; }
        public Hero hero { get; set; }
        public Body body { get; set; }
        public Footer footer { get; set; }
    }

}