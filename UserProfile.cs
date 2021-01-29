using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {
        public string Transport { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Attachment Picture { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Interest { get; set; }
        public string Location { get; set; }
    }
}