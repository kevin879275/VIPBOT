﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Text.RegularExpressions;

using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Imgur;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples
{

  public class DispatchBot : ActivityHandler { 
    private readonly ILogger<DispatchBot> _logger;
    private readonly IBotServices _botServices;

    private static readonly HttpClient client = new HttpClient();
    private static SQL_Database db = new SQL_Database();


    protected BotState ConversationState;

    protected BotState UserState;
    //protected readonly StartDialog Dialog;
    private static Dictionary<string, StartDialog> askFirstState = new Dictionary<string, StartDialog>();
    private static Dictionary<string, string> dialogState = new Dictionary<string, string>();

    private readonly string[] _cards = {

        //Path.Combine (".", "Cards", "Covid19Status.json"),
        //Path.Combine (".", "Cards", "GlobalStatus.json"),
    };

    public DispatchBot(IBotServices botServices, ILogger<DispatchBot> logger ,ConversationState conversationState, UserState userState)
    {
      _logger = logger;
      _botServices = botServices;
      ConversationState = conversationState;
      UserState = userState;
    }


    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
      await base.OnTurnAsync(turnContext, cancellationToken);

      // Save any state changes that might have occurred during the turn.
      await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
      await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            //await Dialog.BeginDialogAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            if (dialogState[turnContext.Activity.Recipient.Id] == "Buy")
            {
                var conversationStateAccessors = ConversationState.CreateProperty<BuyFlow>(nameof(BuyFlow));
                var flow = await conversationStateAccessors.GetAsync(turnContext, () => new BuyFlow(), cancellationToken);

                var userStateAccessors = UserState.CreateProperty<BuyItem>(nameof(BuyItem));
                var item = await userStateAccessors.GetAsync(turnContext, () => new BuyItem(), cancellationToken);

                await FillOutBuyItemAsync(flow, item, turnContext, cancellationToken);
                if (flow.LastQuestionAsked == BuyFlow.Question.None)
                {
                    dialogState[turnContext.Activity.Recipient.Id] = "None";
                    db.Insert_tabBought_List(turnContext.Activity.Recipient.Id, item.iId, item.quantiy);
                }
            }
            else if (askFirstState[turnContext.Activity.Recipient.Id].flow.LastQuestionAsked != StartConversationFlow.Question.End)
            {
                await askFirstState[turnContext.Activity.Recipient.Id].StartFlow(turnContext, cancellationToken);
            }
            else if (dialogState[turnContext.Activity.Recipient.Id] == "Sell")
            {
                var conversationStateAccessors = ConversationState.CreateProperty<SellFlow>(nameof(SellFlow));
                var flow = await conversationStateAccessors.GetAsync(turnContext, () => new SellFlow(), cancellationToken);

                var userStateAccessors = UserState.CreateProperty<SellItem>(nameof(SellItem));
                var item = await userStateAccessors.GetAsync(turnContext, () => new SellItem(), cancellationToken);

                await FillOutSellItemAsync(flow, item, turnContext, cancellationToken);
                if (flow.LastQuestionAsked == SellFlow.Question.None)
                {
                    dialogState[turnContext.Activity.Recipient.Id] = "None";
                    //db.Insert_tabBought_List(turnContext.Activity.Recipient.Id, item.iId, item.quantiy);
                }
            }
            else
            {
                var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
                // Top intent tell us which cognitive service to use.
                var topIntent = recognizerResult.GetTopScoringIntent();

                // Next, we call the dispatcher with the top intent.
                await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
            }
        }
    
    private static int itemNow = 0;
    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
      dialogState[turnContext.Activity.Recipient.Id] = "None";
      foreach (var member in membersAdded)
      {
        if (member.Id != turnContext.Activity.Recipient.Id)
        {
            askFirstState[turnContext.Activity.Recipient.Id] = new StartDialog();
            await SendFirstActionsAsync(turnContext, cancellationToken);
            //db.Insert_tabUser(turnContext.Activity.Recipient.Id, "新竹市東區", "[\"天竺鼠車車\",\"車車天竺鼠\"]");
            db.Insert_tabItem(itemNow.ToString(), "now", "cart", "", "selling", 5, "天竺鼠車車", "新竹市東區", turnContext.Activity.Recipient.Id, 99999);
            itemNow++;
        }
      }
    }




    private static async Task SendFirstActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        await turnContext.SendActivityAsync(MessageFactory.Text("您好，本機器人提供鄰近區域服務、物品買賣仲介。"), cancellationToken);
        await askFirstState[turnContext.Activity.Recipient.Id].StartFlow(turnContext, cancellationToken);
    }

    private int getNumberInString(string s)
    {
      return Int32.Parse(Regex.Match(s, @"\d+").Value);
    }



    private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
    {
      switch (intent)
      {
        case "l_BuySell":
          await ProcessVipBotAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
          break;
        case "q_BuySell":
          await ProcessSampleQnAAsync(turnContext, cancellationToken);
          break;
        default:
          _logger.LogInformation($"機器人無法辨識您");
          await turnContext.SendActivityAsync(MessageFactory.Text($"機器人無法辨識您"), cancellationToken);
          break;
      }
    }

    private async Task ProcessVipBotAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
    {

      // Retrieve LUIS result for Process Automation.
      var result = luisResult.ConnectedServiceResult;
      var topIntent = result.TopScoringIntent.Intent;

      if (topIntent == "Number")
      {

      }
      else if (topIntent == "Buy")
      {

                var conversationStateAccessors = ConversationState.CreateProperty<BuyFlow>(nameof(BuyFlow));
                var flow = await conversationStateAccessors.GetAsync(turnContext, () => new BuyFlow(), cancellationToken);

                var userStateAccessors = UserState.CreateProperty<BuyItem>(nameof(BuyItem));
                var item = await userStateAccessors.GetAsync(turnContext, () => new BuyItem(), cancellationToken);

                await FillOutBuyItemAsync(flow, item, turnContext, cancellationToken);
                dialogState[turnContext.Activity.Recipient.Id] = "Buy";
                //var qm = result.Entities.SingleOrDefault(s => s.Type == "Quantity math") ?? result.Entities.SingleOrDefault(s => s.Type == "Measure Quantity");

                //var inum = result.Entities.SingleOrDefault(s => s.Type == "ItemNumber");
        //if (qm == null || inum == null)
        //{
        //  dialogState[turnContext.Activity.Recipient.Id] = true;
        //  //await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        //}
        //else
        //{
        //  int q = getNumberInString(qm.Entity);
        //  int inu = getNumberInString(inum.Entity);

        //  string uid = turnContext.Activity.Recipient.Id;
          //int amount = db.Select_tabItem(inu.ToString());
          //if (amount == 0)
          //{
          //  turnContext.SendActivityAsync(MessageFactory.Text("沒有這個物品id優!!!"));
          //  return;
          //}

          //if (q <= amount)
          //{
          //  db.Insert_tabBought_List(uid, inu.ToString(), q);
          //  int remain = amount - q;
          //  string sta = remain > 0 ? "on sell" : "sold";
          //  db.update_tabItem(sta, inu.ToString(), remain);

          //turnContext.SendActivityAsync(MessageFactory.Text($"庫存剩餘:{db.Select_tabItem(inu.ToString())}"));
          //}
          //else
          //      turnContext.SendActivityAsync(MessageFactory.Text("庫存不足瞜!!!"));

      }
      else if (topIntent == "Sell")
      {
                var conversationStateAccessors = ConversationState.CreateProperty<SellFlow>(nameof(SellFlow));
                var flow = await conversationStateAccessors.GetAsync(turnContext, () => new SellFlow(), cancellationToken);

                var userStateAccessors = UserState.CreateProperty<SellItem>(nameof(SellItem));
                var item = await userStateAccessors.GetAsync(turnContext, () => new SellItem(), cancellationToken);

                await FillOutSellItemAsync(flow, item, turnContext, cancellationToken);
                dialogState[turnContext.Activity.Recipient.Id] = "Sell";

        //var mon = result.Entities.SingleOrDefault(s => s.Type == "builtin.currency");
        //var q = result.Entities.SingleOrDefault(s => s.Type == "Quantity math") ?? result.Entities.SingleOrDefault(s => s.Type == "Measure Quantity");
        //if (mon == null || q == null)
        //{
        //  //dialog
        //}
        //else
        //{
        //  int money = getNumberInString(mon.Entity);
        //  int quantity = getNumberInString(q.Entity);
        //  string other = result.Query;
        //  foreach (var entity in result.Entities)
        //  {
        //    other = other.Replace(entity.Entity, "");
        //  }
        //  other = other.Replace(" ", "");
        //  string name = "no name";
        //  if (other.Length > 0)
        //    name = other;
        //  //db.Insert_tabItem(itemNow.ToString(), DateTime.Now.ToString(), "second hand", "[]", "on sell", quantity, name, "新竹市東區", turnContext.Activity.Recipient.Id, money);
        //  // to do get location from user
        //  itemNow++;
        //}


      }
      else
      {
        _logger.LogInformation($"Luis unrecognized intent.");
        await turnContext.SendActivityAsync(MessageFactory.Text($"機器人無法辨識您的輸入!"), cancellationToken);
      }
    }

    private async Task ProcessSampleQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
      _logger.LogInformation("ProcessSampleQnAAsync");

      var results = await _botServices.SampleQnA.GetAnswersAsync(turnContext);
      if (results.Any())
      {
        await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
      }
      else
      {
        await turnContext.SendActivityAsync(MessageFactory.Text("抱歉! 機器人無法回答您的問題"), cancellationToken);
      }
    }


    private static Attachment CreateAdaptiveCardAttachment(string filePath)
    {
      var adaptiveCardJson = File.ReadAllText(filePath);
      var adaptiveCardAttachment = new Attachment()
      {
        ContentType = "application/vnd.microsoft.card.adaptive",
        Content = JsonConvert.DeserializeObject(adaptiveCardJson),
      };
      return adaptiveCardAttachment;
    }

    private static JObject readFileforUpdate_jobj(string filepath)
    {
      var json = File.ReadAllText(filepath);
      var jobj = JsonConvert.DeserializeObject(json);
      JObject Jobj_card = JObject.FromObject(jobj) as JObject;
      return Jobj_card;
    }

    private static Attachment UpdateAdaptivecardAttachment(JObject updateAttch)
    {
      var adaptiveCardAttch = new Attachment()
      {
        ContentType = "application/vnd.microsoft.card.adaptive",
        Content = JsonConvert.DeserializeObject(updateAttch.ToString()),
      };
      return adaptiveCardAttch;
    }

    private static List<Repo> ProcessRepo(string Url)
    {
      var webRequest = WebRequest.Create(Url) as HttpWebRequest;
      List<Repo> repositories = new List<Repo>();
      webRequest.ContentType = "application/json";
      webRequest.UserAgent = "User-Agent";
      using (var s = webRequest.GetResponse().GetResponseStream())
      {
        using (var sr = new StreamReader(s))
        {
          var contributorsAsJson = sr.ReadToEnd();
          repositories = JsonConvert.DeserializeObject<List<Repo>>(contributorsAsJson);
        }
      }
      return repositories;
    }

    // private static List<RepoWorld> ProcessRepoWorld (string Url) {
    //     var webRequest = WebRequest.Create (Url) as HttpWebRequest;
    //     List<RepoWorld> repositories = new List<RepoWorld> ();
    //     webRequest.ContentType = "application/json";
    //     webRequest.UserAgent = "User-Agent";
    //     using (var s = webRequest.GetResponse ().GetResponseStream ()) {
    //         using (var sr = new StreamReader (s)) {
    //             var contributorsAsJson = sr.ReadToEnd ();
    //             repositories = JsonConvert.DeserializeObject<List<RepoWorld>> (contributorsAsJson);
    //         }
    //     }
    //     return repositories;
    // }

    private static JObject ProcessRepoWorldJ(string Url)
    {
      var webRequest = WebRequest.Create(Url) as HttpWebRequest;
      JObject repositories = new JObject();
      webRequest.ContentType = "application/json";
      webRequest.UserAgent = "User-Agent";
      using (var s = webRequest.GetResponse().GetResponseStream())
      {
        using (var sr = new StreamReader(s))
        {
          var contributorsAsJson = sr.ReadToEnd();
          var jobj = JsonConvert.DeserializeObject(contributorsAsJson);
          JObject Jobj_card = JObject.FromObject(jobj) as JObject;

          repositories = JObject.Parse(contributorsAsJson);
        }
      }
      return repositories;
    }
        private static async Task FillOutBuyItemAsync(BuyFlow flow, BuyItem Item, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();
            string message;

            switch (flow.LastQuestionAsked)
            {
                case BuyFlow.Question.None:
                    await turnContext.SendActivityAsync("您好，你要買什麼東西?", null, null, cancellationToken);
                    flow.LastQuestionAsked = BuyFlow.Question.ID;
                    break;
                case BuyFlow.Question.ID:
                    if (ValidateID(input, out var ID, out message))
                    {
                        Item.iId = ID;
                        //await turnContext.SendActivityAsync($"Hi {profile.Name}.", null, null, cancellationToken);
                        await turnContext.SendActivityAsync("數量多少?", null, null, cancellationToken);
                        flow.LastQuestionAsked = BuyFlow.Question.Qua;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
                case BuyFlow.Question.Qua:
                    if (ValidateQua(input, out var Q, out message, Item.iId))
                    {
                        Item.quantiy = Q;
                        //await turnContext.SendActivityAsync($"I have your age as {profile.Age}.", null, null, cancellationToken);
                        await turnContext.SendActivityAsync("感謝您的購買!", null, null, cancellationToken);
                        flow.LastQuestionAsked = BuyFlow.Question.None;
                        Item = new BuyItem();
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }

                //case ConversationFlow.Question.Date:
                //    if (ValidateDate(input, out var date, out message))
                //    {
                //        profile.Date = date;
                //        await turnContext.SendActivityAsync($"Your cab ride to the airport is scheduled for {profile.Date}.");
                //        await turnContext.SendActivityAsync($"Thanks for completing the booking {profile.Name}.");
                //        await turnContext.SendActivityAsync($"Type anything to run the bot again.");
                //        flow.LastQuestionAsked = ConversationFlow.Question.None;
                //        profile = new UserProfile();
                //        break;
                //    }
                //    else
                //    {
                //        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                //        break;
                //    }
            }
        }

        private static bool ValidateID(string input, out string ID, out string message)
        {
            ID = null;
            message = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                message = "Please enter a name that contains at least one character.";
            }
            else
            {
                ID = input.Trim();
            }
            int amount = db.Select_tabItem(ID);
            if (amount == 0)
            {
                message = "沒有這個物品呦!!!";
            }
            return message is null;
        }

        private static bool ValidateQua(string input, out int Qua, out string message, string ID)
        {
            Qua = 0;
            message = null;
            int amount = db.Select_tabItem(ID);
            // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
            try
            {
                // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
                // The recognizer returns a list of potential recognition results, if any.

                var results = NumberRecognizer.RecognizeNumber(input, Culture.English);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "value" entry contains the processed string.
                    if (result.Resolution.TryGetValue("value", out var value))
                    {
                        Qua = Convert.ToInt32(value);
                        if (Qua > 0)
                        {
                            if (Qua <= amount)
                            {
                                int remain = amount - Qua;
                                string sta = remain > 0 ? "on sell" : "sold";
                                db.update_tabItem(sta, ID, remain);
                                //message = $"庫存剩餘:{db.Select_tabItem(ID)}";
                                return true;
                            }
                            else
                            {
                                message = $"庫存不足瞜!!!庫存剩餘:{db.Select_tabItem(ID)}，請再輸入一次";
                            }
                        }
                        else
                        {
                            message = "輸入大於0";
                            break;
                        }
                    
                    }
                }
            }
            catch
            {
                message = "輸入大於0";
            }

            return message is null;
        }

        private static async Task FillOutSellItemAsync(SellFlow flow, SellItem Item, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();
            string message;

            switch (flow.LastQuestionAsked)
            {
                case SellFlow.Question.None:
                    await turnContext.SendActivityAsync("您好，你要賣什麼東西(請上傳圖片)?", null, null, cancellationToken);
                    flow.LastQuestionAsked = SellFlow.Question.imageSrc;
                    break;
                case SellFlow.Question.imageSrc:
                    if (ValidateImage(input, out var image, out message))
                    {
                        Item.imageSrc = image;
                        await turnContext.SendActivityAsync("您好，你要賣什麼類型?", null, null, cancellationToken);
                        Item.imageSrc = image;
                        var reply = MessageFactory.Text("請選擇下列類型?");
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "3C產品", Type = ActionTypes.ImBack, Value = "3C產品"},
                                new CardAction() { Title = "電腦週邊", Type = ActionTypes.ImBack, Value = "電腦週邊"},
                                new CardAction() { Title = "食品", Type = ActionTypes.ImBack, Value = "食品"},
                                new CardAction() { Title = "樂器", Type = ActionTypes.ImBack, Value = "樂器"},
                                new CardAction() { Title = "書籍", Type = ActionTypes.ImBack, Value = "書籍"},
                                new CardAction() { Title = "票券", Type = ActionTypes.ImBack, Value = "票券"},
                                new CardAction() { Title = "其他", Type = ActionTypes.ImBack, Value = "其他"},
                            },
                        };
                        flow.LastQuestionAsked = SellFlow.Question.type;
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
              case SellFlow.Question.type:
                    if (ValidateType(input, out var type, out message))
                    {
                        Item.type = type;
                        await turnContext.SendActivityAsync("您好，請稍微描述您的物品?", null, null, cancellationToken);
                        flow.LastQuestionAsked = SellFlow.Question.discription;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
                case SellFlow.Question.discription:
                    if (ValidateDiscription(input, out var description, out message))
                    {
                        Item.description = description;
                        await turnContext.SendActivityAsync("您的定價為多少呢?", null, null, cancellationToken);
                        flow.LastQuestionAsked = SellFlow.Question.price;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }

                case SellFlow.Question.price:
                    if (ValidatePrice(input, out var price, out message))
                    {
                        Item.price = price;
                        var reply = MessageFactory.Text("請確認您的商品?");
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "是", Type = ActionTypes.ImBack, Value = "是"},
                                new CardAction() { Title = "否", Type = ActionTypes.ImBack, Value = "否"},
                            },
                        };
                        flow.LastQuestionAsked = SellFlow.Question.Check;
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
                case SellFlow.Question.Check:
                    if (ValidateCheck(input, out var check, out message))
                    {
                        Item.ownerUserId = turnContext.Activity.Recipient.Id;
                        Item.time = DateTime.Now.ToString();
                        await turnContext.SendActivityAsync("感謝您，物品已成功登錄", null, null, cancellationToken);
                        Item = new SellItem();
                        flow.LastQuestionAsked = SellFlow.Question.None;
                        break;
                    }
                    else
                    {
                        flow.LastQuestionAsked = SellFlow.Question.None;
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
            }
        }

        private static bool ValidateImage(string input, out string image, out string message)
        {
            image = null;
            message = null;
            //image = Imgur.Imgur.UploadSrc(input);

            return message is null;
        }

        private static bool ValidateType(string input, out string type, out string message)
        {
            type = "";
            message = null;
            return message is null;
        }

        private static bool ValidateDiscription(string input, out string discription, out string message)
        {
            discription = "";
            message = null;
            if (string.IsNullOrWhiteSpace(input))
            {
                message = "請至少輸入一個字";
            }
            else
            {
                discription = input.Trim();
            }
            return message is null;
        }

        private static bool ValidatePrice(string input, out int price, out string message)
        {
            price = 0;
            message = null;
            // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
            try
            {
                // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
                // The recognizer returns a list of potential recognition results, if any.

                var results = NumberRecognizer.RecognizeNumber(input, Culture.English);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "value" entry contains the processed string.
                    if (result.Resolution.TryGetValue("value", out var value))
                    {
                        price = Convert.ToInt32(value);
                        if (price > 0)
                        {
                            return true;
                        }
                        else
                        {
                            message = "商品金額需大於0";
                            break;
                        }
                    }
                }
            }
            catch
            {
                message = "商品金額需大於0";
            }
            return message is null;
        }
        private static bool ValidateCheck(string input, out string check, out string message)
        {
            message = null;
            check = input;
            if(check == "是")
            {

            }
            else
            {
                message = "交易已取消";
            }
            return message is null;
        }
        private static double getDistance(float lat1, float long1, float lat2, float long2)
        {
            var R = 6371;

            double rad(double x)
            {
                return x * Math.PI / 180;
            }

            var dLat = rad(lat2 - lat1);

            var dLong = rad(long2 - long1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(rad(lat1)) * Math.Cos(rad(lat2)) *
                    Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = R * c;

            return d;
        }
    }

}


