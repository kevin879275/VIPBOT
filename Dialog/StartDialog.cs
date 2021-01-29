//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Dialogs.Choices;
//using Microsoft.Bot.Connector;
//using Microsoft.Bot.Schema;
//using Newtonsoft.Json;

//namespace Microsoft.BotBuilderSamples
//{
//    //public class StartDialog : ComponentDialog
//    //{
//    //    private readonly IStatePropertyAccessor<User> _userAccessor;

//    //    public StartDialog(UserState userState)
//    //        : base(nameof(StartDialog))
//    //    {
//    //        _userAccessor = userState.CreateProperty<User>("User");

//    //        // This array defines how the Waterfall will execute.
//    //        var waterfallSteps = new WaterfallStep[]
//    //        {
//    //            LocationStepAsync,
//    //            InterestStepAsync,
//    //        };

//    //        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
//    //        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
//    //        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
//    //        AddDialog(new TextPrompt(nameof(TextPrompt)));

//    //        // The initial child Dialog to run.
//    //        InitialDialogId = nameof(WaterfallDialog);
//    //    }

//    //    private static async Task<DialogTurnResult> LocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//    //    {
//    //        return await stepContext.PromptAsync(nameof(TextPrompt),
//    //            new PromptOptions
//    //            {
//    //                Prompt = MessageFactory.Text("�п�ܱz����m��T"),
//    //            }, cancellationToken);
//    //    }

//    //    private static async Task<DialogTurnResult> InterestStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//    //    {
//    //        return await stepContext.PromptAsync(nameof(ChoicePrompt),
//    //            new PromptOptions
//    //            {
//    //                Prompt = MessageFactory.Text("�]�w�z����������"),
//    //                Choices = ChoiceFactory.ToChoices(new List<string> { "���u���|", "3C���~", "�q���P��", "���~", "�־�", "���y", "����" }),
//    //            }, cancellationToken);
//    //    }

//    //}
//    public class StartDialog
//    {
//        public async Task StartFlow(ITurnContext<IMessageActivity> turnContext, ConversationState _conversationState, UserState  _userState, CancellationToken cancellationToken)
//        {
//            //var conversationStateAccessors = _conversationState.CreateProperty<StartConversationFlow>(nameof(StartConversationFlow));
//            //var flow = await conversationStateAccessors.GetAsync(turnContext, () => new StartConversationFlow(), cancellationToken);

//            //var userStateAccessors = _userState.CreateProperty<User>(nameof(User));
//            //var profile = await userStateAccessors.GetAsync(turnContext, () => new User(), cancellationToken);

//            //await FillOutUserAsync(flow, profile, turnContext, cancellationToken);

//            //// Save changes.
//            //await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
//            //await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
//        }

//        private async Task FillOutUserAsync(StartConversationFlow flow, User profile, ITurnContext turnContext, CancellationToken cancellationToken)
//        {
//            var reply = MessageFactory.Text("�п�ܱz����������");
//            reply.SuggestedActions = new SuggestedActions()
//            {

//                Actions = new List<CardAction>()
//                {
//                    new CardAction() { Title = "���u���|", Type = ActionTypes.ImBack, Value = "���u���|"},
//                    new CardAction() { Title = "3C���~", Type = ActionTypes.ImBack, Value = "3C���~"},
//                    new CardAction() { Title = "�q���P��", Type = ActionTypes.ImBack, Value = "�q���P��"},
//                    new CardAction() { Title = "���~", Type = ActionTypes.ImBack, Value = "���~"},
//                    new CardAction() { Title = "�־�", Type = ActionTypes.ImBack, Value = "�־�"},
//                    new CardAction() { Title = "���y", Type = ActionTypes.ImBack, Value = "���y"},
//                    new CardAction() { Title = "����", Type = ActionTypes.ImBack, Value = "����"},
//                },
//            };

//            switch (flow.LastQuestionAsked)
//            {
//                case StartConversationFlow.Question.Begin:
//                    await turnContext.SendActivityAsync("�п�ܱz����m��T", null, null, cancellationToken);
//                    flow.LastQuestionAsked = StartConversationFlow.Question.Location;
//                    break;
//                case StartConversationFlow.Question.Location:
//                    if (ValidateLocation(turnContext, out var location))
//                    {
//                        profile.Location = location;
//                        await turnContext.SendActivityAsync(reply, cancellationToken);
//                        flow.LastQuestionAsked = StartConversationFlow.Question.Interest;
//                        break;
//                    }
//                    else
//                    {
//                        await turnContext.SendActivityAsync($"�ܩ�p�A�ШϥΥ��T����m��T", null, null, cancellationToken);
//                        break;
//                    }
//                case StartConversationFlow.Question.Interest:
//                    if (ValidateInterest(turnContext, out var interest))
//                    {
//                        profile.Interest = interest;
//                        flow.LastQuestionAsked = StartConversationFlow.Question.End;
//                        break;
//                    }
//                    else
//                    {
//                        reply.Text = "�ܩ�p�A�Цb�]�w�@��";
//                        await turnContext.SendActivityAsync(reply, cancellationToken);
//                        break;
//                    }
//            }
//        }

//        private bool ValidateLocation(ITurnContext turnContext, out string location)
//        {
//            location = null;
//            var channelData = ((ITurnContext<IMessageActivity>)turnContext).Activity.ChannelData;
//            string msgType = channelData["type"];
//            if (msgType == "location")
//            {
//                location = JsonConvert.SerializeObject(channelData);
//            }
//            return location is not null;
//        }

//        private bool ValidateInterest(ITurnContext turnContext, out string interest)
//        {
//            interest = null;
//            Dictionary<string, string> interestDict =
//            new Dictionary<string, string>()
//            {
//                {"���u���|", "���u���|"}, {"3C���~", "3C���~"},
//                {"�q���P��", "�q���P��"}, {"���~", "���~"},
//                {"�־�", "�־�"}, {"���y", "���y"},
//                {"����", "����"}
//            };
//            if (interestDict.ContainsKey(turnContext.Activity.Text))
//            {
//                interest = turnContext.Activity.Text;
//            }
//            return interest is not null;
//        }

//    }
//}
