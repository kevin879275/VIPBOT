using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private static SQL_Database db;

        public StartDialog()
        {
            profile = new User();
            flow = new StartConversationFlow();
            db = new SQL_Database();
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
                    new CardAction() { Title = $"3C產品", Type = ActionTypes.ImBack, Value = $"3C產品"},
                    new CardAction() { Title = $"電腦周邊", Type = ActionTypes.ImBack, Value = $"電腦周邊"},
                    new CardAction() { Title = $"食品", Type = ActionTypes.ImBack, Value = $"食品"},
                    new CardAction() { Title = $"樂器", Type = ActionTypes.ImBack, Value = $"樂器"},
                    new CardAction() { Title = $"書籍", Type = ActionTypes.ImBack, Value = $"書籍"},
                    new CardAction() { Title = $"票券", Type = ActionTypes.ImBack, Value = $"票券"},
                    new CardAction() { Title = $"其他", Type = ActionTypes.ImBack, Value = $"其他"},
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
                        profile.location = location;
                        if (turnContext.Activity.ChannelId != "line")
                        {
                            profile.UserId = turnContext.Activity.Recipient.Id;
                        }
                        else
                        {
                            profile.UserId = turnContext.Activity.From.Id;
                        }
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
                        //db.Insert_tabUser(
                        //    profile.UserId,
                        //    profile.location.Address, 
                        //    profile.Interest,
                        //    profile.location.Latitude, 
                        //    profile.location.Longitude);
                        await turnContext.SendActivityAsync($"已完成輸入，感謝您", null, null, cancellationToken);
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

        private bool ValidateLocation(ITurnContext turnContext, out Location location)
        {
            location = null;
            if (((ITurnContext<IMessageActivity>)turnContext).Activity.ChannelId != "line")
            {
                location = new Location
                {
                    Type = "location",
                    Address = "新竹市東區",
                    Latitude = 35.688806F,
                    Longitude = 139.701739F,
                };
                return true;
            }

            //var channelData = ((ITurnContext<IMessageActivity>)turnContext).Activity.ChannelData;
            var channelData = ((DelegatingTurnContext<IMessageActivity>)turnContext).Activity.ChannelData.ToString();
            try
            {
                var msg = JsonConvert.DeserializeObject<LineLocation>(channelData);
                var msgType = msg.message;
                if (msgType.Type == "location")
                {
                    location = msgType;
                }
            }
            catch
            {
                return false;
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
                {$"票券", $"票券"},{$"其他", $"其他"}
            };
            if (interestDict.ContainsKey(turnContext.Activity.Text))
            {
                interest = turnContext.Activity.Text;
            }
            return interest is not null;
        }

    }
}
