namespace Microsoft.BotBuilderSamples
{
    public class SellFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            imageSrc,
            type,
            time,
            discription,
            location,
            OwnerUserID,
            price,
            None,
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}
