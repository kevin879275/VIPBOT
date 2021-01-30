using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.BotBuilderSamples
{
    public class LineAdapt
    {
        [JsonPropertyName("replyToken")]
        public string replyToken { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("source")]
        public Source source { get; set; }
    }

    public class LineLocation
    {
        [JsonPropertyName("message")]
        public Location message { get; set; }
        [JsonPropertyName("replyToken")]
        public string replyToken { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("source")]
        public Source source { get; set; }
    }

    public class LineImage
    {
        [JsonPropertyName("message")]
        public Image message { get; set; }
        [JsonPropertyName("replyToken")]
        public string replyToken { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        public Source source { get; set; }
    }

    public class Source
    {
        public string type { get; set; }
        public string userId { get; set; }
    }

    public class Image
    {
        public Contentprovider contentProvider { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Contentprovider
    {
        public string type { get; set; }
        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
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