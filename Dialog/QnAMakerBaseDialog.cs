////// Copyright (c) Microsoft Corporation. All rights reserved.
////// Licensed under the MIT License.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.AI.QnA;
//using Microsoft.Bot.Builder.AI.QnA.Dialogs;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Schema;

//namespace Microsoft.BotBuilderSamples.Dialog
//{
//    /// <summary>
//    /// QnAMaker action builder class
//    /// </summary>
//    public class QnAMakerBaseDialog : ComponentDialog
//    {
//        //// Dialog Options parameters
//        //public const string DefaultNoAnswer = "No QnAMaker answers found.";
//        //public const string DefaultCardTitle = "Did you mean:";
//        //public const string DefaultCardNoMatchText = "None of the above.";
//        //public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";

//        //private readonly IBotServices _services;

//        ///// <summary>
//        ///// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
//        ///// Dialog helper to generate dialogs.
//        ///// </summary>
//        ///// <param name="services">Bot Services.</param>
//        //public QnAMakerBaseDialog(IBotServices services) : base()
//        //{
//        //    this._services = services;
//        //}

//        //protected async override Task<IQnAMakerClient> GetQnAMakerClientAsync(DialogContext dc)
//        //{
//        //    return this._services?.SampleQnA;
//        //}

//        //protected override Task<QnAMakerOptions> GetQnAMakerOptionsAsync(DialogContext dc)
//        //{
//        //    return Task.FromResult(new QnAMakerOptions
//        //    {
//        //        ScoreThreshold = DefaultThreshold,
//        //        Top = DefaultTopN,
//        //        QnAId = 0,
//        //        RankerType = "Default",
//        //        IsTest = false
//        //    });
//        //}

//        //protected async override Task<QnADialogResponseOptions> GetQnAResponseOptionsAsync(DialogContext dc)
//        //{
//        //    var noAnswer = (Activity)Activity.CreateMessageActivity();
//        //    noAnswer.Text = DefaultNoAnswer;

//        //    var cardNoMatchResponse = (Activity)MessageFactory.Text(DefaultCardNoMatchResponse);


//        //    var responseOptions = new QnADialogResponseOptions
//        //    {
//        //        ActiveLearningCardTitle = DefaultCardTitle,
//        //        CardNoMatchText = DefaultCardNoMatchText,
//        //        NoAnswer = noAnswer,
//        //        CardNoMatchResponse = cardNoMatchResponse,
//        //    };

//        //    return responseOptions;
//        //}
//        // Dialog Options parameters	        // Dialog Options parameters
//        public const float DefaultThreshold = 0.3F;
//        public const int DefaultTopN = 3;
//        public const string DefaultNoAnswer = "No QnAMaker answers found.";

//        // Card parameters	
//        public const string DefaultCardTitle = "Did you mean:";
//        public const string DefaultCardNoMatchText = "None of the above.";
//        public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";


//        // Define value names for values tracked inside the dialogs.	
//        public const string QnAOptions = "qnaOptions";
//        public const string QnADialogResponseOptions = "qnaDialogResponseOptions";
//        private const string CurrentQuery = "currentQuery";
//        private const string QnAData = "qnaData";
//        private const string QnAContextData = "qnaContextData";
//        private const string PreviousQnAId = "prevQnAId";

//        /// <summary>
//        /// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
//        /// Dialog helper to generate dialogs.
//        /// </summary>
//        /// <param name="configuration">QnA Maker configuration.</param>
//        public QnAMakerBaseDialog(IBotServices configuration) : base(nameof(QnAMakerBaseDialog))
//        {
//            AddDialog(new WaterfallDialog(QnAMakerDialogName)
//                .AddStep(CallGenerateAnswerAsync)
//                .AddStep(CheckForMultiTurnPrompt)
//                .AddStep(DisplayQnAResult));
//            _qnaService = configuration?.SampleQnA ?? throw new ArgumentNullException(nameof(configuration));

//            // The initial child Dialog to run.
//            InitialDialogId = QnAMakerDialogName;
//        }

//        private const string QnAMakerDialogName = "qnamaker-multiturn-dialog";
//        private readonly QnAMaker _qnaService;

//        private static Dictionary<string, object> GetDialogOptionsValue(DialogContext dialogContext)
//        {
//            var dialogOptions = new Dictionary<string, object>();

//            if (dialogContext.ActiveDialog.State["options"] != null)
//            {
//                dialogOptions = dialogContext.ActiveDialog.State["options"] as Dictionary<string, object>;
//            }

//            return dialogOptions;
//        }

//        private async Task<DialogTurnResult> CallGenerateAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            return await stepContext.NextAsync()
//        }

//        private async Task<DialogTurnResult> CheckForMultiTurnPrompt(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {

//        }

//        private async Task<DialogTurnResult> DisplayQnAResult(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {

//        }

//        /// <summary>
//        /// Get multi-turn prompts card.
//        /// </summary>
//        /// <param name="result">Result to be dispalyed as prompts.</param>
//        /// <returns>IMessageActivity.</returns>
//        private static IMessageActivity GetQnAPromptsCardWithoutNoMatch(QueryResult result)
//        {
//            if (result == null)
//            {
//                throw new ArgumentNullException(nameof(result));
//            }

//            var chatActivity = Activity.CreateMessageActivity();
//            chatActivity.Text = result.Answer;
//            var buttonList = new List<CardAction>();

//            // Add all prompt
//            foreach (var prompt in result.Context.Prompts)
//            {
//                buttonList.Add(
//                    new CardAction()
//                    {
//                        Value = prompt.DisplayText,
//                        Type = "imBack",
//                        Title = prompt.DisplayText,
//                    });
//            }

//            var plCard = new HeroCard()
//            {
//                Buttons = buttonList
//            };

//            // Create the attachment.
//            var attachment = plCard.ToAttachment();

//            chatActivity.Attachments.Add(attachment);

//            return chatActivity;
//        }
//    }
//}
