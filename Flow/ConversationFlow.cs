namespace Microsoft.BotBuilderSamples
{
    public class StartConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            Begin,
            Location,
            Interest,
            End, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.Begin;
    }
}