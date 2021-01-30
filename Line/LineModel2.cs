using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.BotBuilderSamples
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Hero2
    {
        public string type { get; set; }
        public string url { get; set; }
        public string align { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public string backgroundColor { get; set; }
    }

    public class Content2
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string align { get; set; }
        public List<object> contents { get; set; }
        public Action action { get; set; }
        public string color { get; set; }
        public string layout { get; set; }
    }

    public class Body2
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<Content> contents { get; set; }
    }

    public class Action2
    {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
    }

    public class Footer2
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public List<Content> contents { get; set; }
    }

    public class Root2
    {
        public string type { get; set; }
        public string size { get; set; }
        public Hero hero { get; set; }
        public Body body { get; set; }
        public Footer footer { get; set; }
    }

}