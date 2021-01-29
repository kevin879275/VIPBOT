﻿namespace Microsoft.BotBuilderSamples
{
    public class BuyFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            ID,
            Qua,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}
