using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    //public class StartDialog : ComponentDialog
    //{
    //    private readonly IStatePropertyAccessor<User> _userAccessor;

    //    public StartDialog(UserState userState)
    //        : base(nameof(StartDialog))
    //    {
    //        _userAccessor = userState.CreateProperty<User>("User");

    //        // This array defines how the Waterfall will execute.
    //        var waterfallSteps = new WaterfallStep[]
    //        {
    //            LocationStepAsync,
    //            InterestStepAsync,
    //        };

    //        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
    //        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
    //        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
    //        AddDialog(new TextPrompt(nameof(TextPrompt)));

    //        // The initial child Dialog to run.
    //        InitialDialogId = nameof(WaterfallDialog);
    //    }

    //    private static async Task<DialogTurnResult> LocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        return await stepContext.PromptAsync(nameof(TextPrompt),
    //            new PromptOptions
    //            {
    //                Prompt = MessageFactory.Text("�п�ܱz����m��T"),
    //            }, cancellationToken);
    //    }

    //    private static async Task<DialogTurnResult> InterestStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        return await stepContext.PromptAsync(nameof(ChoicePrompt),
    //            new PromptOptions
    //            {
    //                Prompt = MessageFactory.Text("�]�w�z����������"),
    //                Choices = ChoiceFactory.ToChoices(new List<string> { "���u���|", "3C���~", "�q���P��", "���~", "�־�", "���y", "����" }),
    //            }, cancellationToken);
    //    }

    //}
    public class StartDialog
    {
        public User profile;
        public StartConversationFlow flow;
        public StartDialog()
        {
            profile = new User();
            flow = new StartConversationFlow();
        }

        public async Task StartFlow(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await FillOutUserAsync(turnContext, cancellationToken);
        }

        private async Task FillOutUserAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text($"請設定您的興趣項目");
            reply.SuggestedActions = new SuggestedActions()
            {

                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = $"打工機會", Type = ActionTypes.ImBack, Value = $"打工機會"},
                    new CardAction() { Title = $"3C產品", Type = ActionTypes.ImBack, Value = $"3C產品"},
                    new CardAction() { Title = $"電腦周邊", Type = ActionTypes.ImBack, Value = $"電腦周邊"},
                    new CardAction() { Title = $"食品", Type = ActionTypes.ImBack, Value = $"食品"},
                    new CardAction() { Title = $"樂器", Type = ActionTypes.ImBack, Value = $"樂器"},
                    new CardAction() { Title = $"書籍", Type = ActionTypes.ImBack, Value = $"書籍"},
                    new CardAction() { Title = $"票券", Type = ActionTypes.ImBack, Value = $"票券"},
                },
            };

            switch (flow.LastQuestionAsked)
            {
                case StartConversationFlow.Question.Begin:
                    await turnContext.SendActivityAsync($"請輸入您的位置資訊", null, null, cancellationToken);
                    flow.LastQuestionAsked = StartConversationFlow.Question.Location;
                    break;
                case StartConversationFlow.Question.Location:
                    if (ValidateLocation(turnContext, out var location))
                    {
                        profile.Location = location;
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                        flow.LastQuestionAsked = StartConversationFlow.Question.Interest;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"輸入有誤，請再輸入一次您的位置資訊", null, null, cancellationToken);
                        break;
                    }
                case StartConversationFlow.Question.Interest:
                    if (ValidateInterest(turnContext, out var interest))
                    {
                        profile.Interest = interest;
                        flow.LastQuestionAsked = StartConversationFlow.Question.End;
                        break;
                    }
                    else
                    {
                        reply.Text = $"輸入有誤，請再設定您的興趣項目";
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                        break;
                    }
            }
        }

        private bool ValidateLocation(ITurnContext turnContext, out string location)
        {
            location = null;
            var channelData = ((ITurnContext<IMessageActivity>)turnContext).Activity.ChannelData;
            string msgType = channelData["type"];
            if (msgType == "location")
            {
                location = JsonConvert.SerializeObject(channelData);
            }
            return location is not null;
        }

        private bool ValidateInterest(ITurnContext turnContext, out string interest)
        {
            interest = null;
            Dictionary<string, string> interestDict =
            new Dictionary<string, string>()
            {
                {$"打工機會", $"打工機會"}, {$"3C產品", $"3C產品"},
                {$"電腦周邊", $"電腦周邊"}, {$"食品", $"食品"},
                {$"樂器", $"樂器"}, {$"書籍", $"書籍"},
                {$"票券", $"票券"}
            };
            if (interestDict.ContainsKey(turnContext.Activity.Text))
            {
                interest = turnContext.Activity.Text;
            }
            return interest is not null;
        }

    }
}