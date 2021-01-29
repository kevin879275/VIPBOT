//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using ChatState;
//using System.Data.SqlClient;
//using System.Text.RegularExpressions;

//using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Schema;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Imgur;
//using Microsoft.Bot.Builder.Dialogs;

//namespace Microsoft.BotBuilderSamples
//{

//  public class DispatchBot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
//  {
//    private readonly ILogger<DispatchBot<T>> _logger;
//    private readonly IBotServices _botServices;

//    private static readonly HttpClient client = new HttpClient();
//    private static SQL_Database db = new SQL_Database();


//    protected static BotState ConversationState;

//    protected static BotState UserState;
//    protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
//    private static Dictionary<string, bool> dialogState = new Dictionary<string, bool>();

//    private static Dictionary<string, ChatState.ChatState> chatStats = new Dictionary<string, ChatState.ChatState>();
//    private readonly string[] _cards = {

//        //Path.Combine (".", "Cards", "Covid19Status.json"),
//        //Path.Combine (".", "Cards", "GlobalStatus.json"),
//    };

//    public DispatchBot(IBotServices botServices, ILogger<DispatchBot<T>> logger, T dialog, ConversationState conversationState, UserState userState)
//    {
//      _logger = logger;
//      _botServices = botServices;
//      Dialog = dialog;
//      ConversationState = conversationState;
//      UserState = userState;
//    }


//    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
//    {
//      await base.OnTurnAsync(turnContext, cancellationToken);

//      // Save any state changes that might have occurred during the turn.
//      await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
//      await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
//    }

//    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
//    {
//      // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
//      //await Dialog.BeginDialogAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
//      string id = turnContext.Activity.Recipient.Id;
//      ChatState.ChatState chatStat = chatStats[id];
//      if (chatStat.stage == "luis")
//        if (dialogState[turnContext.Activity.Recipient.Id] == true)
//        {
//          await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
//        }
//        else
//        {
//          var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
//          // Top intent tell us which cognitive service to use.
//          var topIntent = recognizerResult.GetTopScoringIntent();

//          // Next, we call the dispatcher with the top intent.
//          await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
//        }
//      else if (chatStat.stage == "buy")
//      {
//        if (chatStat.inputKeyNow != "")
//        {
//          string key = chatStat.inputKeyNow;
//          if (key == "location")
//          {

//          }
//          else if (key == "image")
//          {

//          }
//          else
//          {
//            chatStat.userInputs[key] = turnContext.Activity.Text;
//          }

//        }
//        var keyRequired = new Dictionary<string, string>{
//          {"money","請輸入價格"},
//          {"quantity","請輸入數量"},
//          {"image","請輸入商品圖片"}
//        };
//        foreach (var key in keyRequired.Keys)
//        {
//          if (!chatStat.userInputs.ContainsKey(key))
//          {
//            chatStat.inputKeyNow = key;
//            return;
//          }
//        }



//      }
//      else if (chatStat.stage == "sell")
//      {

//      }
//    }
//    private static int itemNow = 0;
//    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
//    {
//      dialogState[turnContext.Activity.Recipient.Id] = false;
//      foreach (var member in membersAdded)
//      {
//        if (member.Id != turnContext.Activity.Recipient.Id)
//        {
//          await SendSuggestedActionsAsync(turnContext, cancellationToken);
//          string id = turnContext.Activity.Recipient.Id;
//          db.Insert_tabUser(id, "新竹市東區", "[\"天竺鼠車車\",\"車車天竺鼠\"]");
//          chatStats[id] = new ChatState.ChatState();
//          //db.Insert_tabItem(itemNow.ToString(), "now", "cart", "", "selling", 5, "天竺鼠車車", "新竹市東區", turnContext.Activity.Recipient.Id, 99999);
//          itemNow++;
//        }
//      }
//    }




//    private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
//    {
//      await turnContext.SendActivityAsync(MessageFactory.Text("您好，本機器人提供鄰近區域服務、物品買賣仲介。"), cancellationToken);
//      //var startPos = new StartDialog();
//      // await startPos.StartFlow(turnContext, ConversationState, UserState, cancellationToken);
//    }



//    private int getNumberInString(string s)
//    {
//      return Int32.Parse(Regex.Match(s, @"\d+").Value);
//    }



//    private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
//    {
//      switch (intent)
//      {
//        case "l_BuySell":
//          await ProcessVipBotAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
//          break;
//        case "q_BuySell":
//          await ProcessSampleQnAAsync(turnContext, cancellationToken);
//          break;
//        default:
//          _logger.LogInformation($"機器人無法辨識您");
//          await turnContext.SendActivityAsync(MessageFactory.Text($"機器人無法辨識您"), cancellationToken);
//          break;
//      }
//    }

//    private async Task ProcessVipBotAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
//    {
//      string id = turnContext.Activity.Recipient.Id;
//      ChatState.ChatState chatStat = chatStats[id];
//      // Retrieve LUIS result for Process Automation.
//      var result = luisResult.ConnectedServiceResult;
//      var topIntent = result.TopScoringIntent.Intent;

//      if (topIntent == "Number")
//      {

//      }
//      else if (topIntent == "Buy")
//      {

//        //var qm = result.Entities.SingleOrDefault(s => s.Type == "Quantity math") ?? result.Entities.SingleOrDefault(s => s.Type == "Measure Quantity");

//        //var inum = result.Entities.SingleOrDefault(s => s.Type == "ItemNumber");
//        //if (qm == null || inum == null)
//        //{
//        //  dialogState[turnContext.Activity.Recipient.Id] = true;
//        //  await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
//        //}
//        //else
//        //{
//        //  int q = getNumberInString(qm.Entity);
//        //  int inu = getNumberInString(inum.Entity);

//        //  string uid = turnContext.Activity.Recipient.Id;
//        //  int amount = db.Select_tabItem(inu.ToString());
//        //  if (amount == 0)
//        //  {
//        //    await turnContext.SendActivityAsync(MessageFactory.Text("沒有這個物品id優!!!"));
//        //    return;
//        //  }

//        //  if (q <= amount)
//        //  {
//        //    db.Insert_tabBought_List(uid, inu.ToString(), q);
//        //    int remain = amount - q;
//        //    string sta = remain > 0 ? "on sell" : "sold";
//        //    db.update_tabItem(sta, inu.ToString(), remain);

//        //    await turnContext.SendActivityAsync(MessageFactory.Text($"庫存剩餘:{db.Select_tabItem(inu.ToString())}"));
//        //  }
//        //  else
//        //    await turnContext.SendActivityAsync(MessageFactory.Text("庫存不足瞜!!!"));
//        //}

//      }
//      else if (topIntent == "Sell")
//      {
//        var mon = result.Entities.SingleOrDefault(s => s.Type == "builtin.currency");
//        var q = result.Entities.SingleOrDefault(s => s.Type == "Quantity math") ?? result.Entities.SingleOrDefault(s => s.Type == "Measure Quantity");
//        if (mon == null || q == null)
//        {
//          chatStat.stage = "Sell";
//          if (mon != null)
//            chatStat.userInputs["money"] = mon.Entity;
//          if (q != null)
//            chatStat.userInputs["quantity"] = q.Entity;
//          //dialog
//        }
//        else
//        {
//          int money = getNumberInString(mon.Entity);
//          int quantity = getNumberInString(q.Entity);
//          string other = result.Query;
//          foreach (var entity in result.Entities)
//          {
//            other = other.Replace(entity.Entity, "");
//          }
//          other = other.Replace(" ", "");
//          string name = "no name";
//          if (other.Length > 0)
//            name = other;
//          //db.Insert_tabItem(itemNow.ToString(), DateTime.Now.ToString(), "second hand", "[]", "on sell", quantity, name, "新竹市東區", turnContext.Activity.Recipient.Id, money);
//          // to do get location from user
//          itemNow++;
//        }


//      }
//      else
//      {
//        _logger.LogInformation($"Luis unrecognized intent.");
//        await turnContext.SendActivityAsync(MessageFactory.Text($"機器人無法辨識您的輸入!"), cancellationToken);
//      }
//    }

//    private async Task ProcessSampleQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
//    {
//      _logger.LogInformation("ProcessSampleQnAAsync");

//      var results = await _botServices.SampleQnA.GetAnswersAsync(turnContext);
//      if (results.Any())
//      {
//        await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
//      }
//      else
//      {
//        await turnContext.SendActivityAsync(MessageFactory.Text("抱歉! 機器人無法回答您的問題"), cancellationToken);
//      }
//    }


//    private static Attachment CreateAdaptiveCardAttachment(string filePath)
//    {
//      var adaptiveCardJson = File.ReadAllText(filePath);
//      var adaptiveCardAttachment = new Attachment()
//      {
//        ContentType = "application/vnd.microsoft.card.adaptive",
//        Content = JsonConvert.DeserializeObject(adaptiveCardJson),
//      };
//      return adaptiveCardAttachment;
//    }

//    private static JObject readFileforUpdate_jobj(string filepath)
//    {
//      var json = File.ReadAllText(filepath);
//      var jobj = JsonConvert.DeserializeObject(json);
//      JObject Jobj_card = JObject.FromObject(jobj) as JObject;
//      return Jobj_card;
//    }

//    private static Attachment UpdateAdaptivecardAttachment(JObject updateAttch)
//    {
//      var adaptiveCardAttch = new Attachment()
//      {
//        ContentType = "application/vnd.microsoft.card.adaptive",
//        Content = JsonConvert.DeserializeObject(updateAttch.ToString()),
//      };
//      return adaptiveCardAttch;
//    }

//    private static List<Repo> ProcessRepo(string Url)
//    {
//      var webRequest = WebRequest.Create(Url) as HttpWebRequest;
//      List<Repo> repositories = new List<Repo>();
//      webRequest.ContentType = "application/json";
//      webRequest.UserAgent = "User-Agent";
//      using (var s = webRequest.GetResponse().GetResponseStream())
//      {
//        using (var sr = new StreamReader(s))
//        {
//          var contributorsAsJson = sr.ReadToEnd();
//          repositories = JsonConvert.DeserializeObject<List<Repo>>(contributorsAsJson);
//        }
//      }
//      return repositories;
//    }

//    // private static List<RepoWorld> ProcessRepoWorld (string Url) {
//    //     var webRequest = WebRequest.Create (Url) as HttpWebRequest;
//    //     List<RepoWorld> repositories = new List<RepoWorld> ();
//    //     webRequest.ContentType = "application/json";
//    //     webRequest.UserAgent = "User-Agent";
//    //     using (var s = webRequest.GetResponse ().GetResponseStream ()) {
//    //         using (var sr = new StreamReader (s)) {
//    //             var contributorsAsJson = sr.ReadToEnd ();
//    //             repositories = JsonConvert.DeserializeObject<List<RepoWorld>> (contributorsAsJson);
//    //         }
//    //     }
//    //     return repositories;
//    // }

//    private static JObject ProcessRepoWorldJ(string Url)
//    {
//      var webRequest = WebRequest.Create(Url) as HttpWebRequest;
//      JObject repositories = new JObject();
//      webRequest.ContentType = "application/json";
//      webRequest.UserAgent = "User-Agent";
//      using (var s = webRequest.GetResponse().GetResponseStream())
//      {
//        using (var sr = new StreamReader(s))
//        {
//          var contributorsAsJson = sr.ReadToEnd();
//          var jobj = JsonConvert.DeserializeObject(contributorsAsJson);
//          JObject Jobj_card = JObject.FromObject(jobj) as JObject;

//          repositories = JObject.Parse(contributorsAsJson);
//        }
//      }
//      return repositories;
//    }
//  }
//}
