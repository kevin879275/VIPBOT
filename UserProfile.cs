namespace Microsoft.BotBuilderSamples
{
    public class UserProfile
    {
        public string Name { get; set; }

        public int? Age { get; set; }

        public string Date { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Interest { get; set; }
        public Location location { get; set; }
    }
}