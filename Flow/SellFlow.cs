namespace Microsoft.BotBuilderSamples
{
    public class SellFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            imageSrc,
            type,
            name,
            time,
            discription,
            location,
            OwnerUserID,
            Qua,
            price,
            Check,
            None,
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}
