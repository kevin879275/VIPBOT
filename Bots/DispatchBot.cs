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

namespace Microsoft.BotBuilderSamples
{

  public class DispatchBot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
  {
    private readonly ILogger<DispatchBot<T>> _logger;
    private readonly IBotServices _botServices;

    private static readonly HttpClient client = new HttpClient();
    //private SQL_Database db = new SQL_Database();


    protected readonly BotState ConversationState;

    protected readonly BotState UserState;
    protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
    private static Dictionary<string, bool> dialogState = new Dictionary<string, bool>();


    private readonly string[] _cards = {

        //Path.Combine (".", "Cards", "Covid19Status.json"),
        //Path.Combine (".", "Cards", "GlobalStatus.json"),
    };

    public DispatchBot(IBotServices botServices, ILogger<DispatchBot<T>> logger, T dialog, ConversationState conversationState, UserState userState)
    {
      _logger = logger;
      _botServices = botServices;
      Dialog = dialog;
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
      if (dialogState[turnContext.Activity.Recipient.Id] == true)
      {
        await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
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

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
      dialogState[turnContext.Activity.Recipient.Id] = false;
      foreach (var member in membersAdded)
      {
        if (member.Id != turnContext.Activity.Recipient.Id)
        {
          await SendSuggestedActionsAsync(turnContext, cancellationToken);
        }
      }
    }




    private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {


      var reply = MessageFactory.Text("您好，本機器人提供鄰近區域服務、物品買賣仲介，第一次使用請傳送您的位置資訊");
      reply.SuggestedActions = new SuggestedActions()
      {

        Actions = new List<CardAction>()
            {
                new CardAction() { Title = "Red", Type = ActionTypes.ImBack, Value = "Red", Image = "https://via.placeholder.com/20/FF0000?text=R", ImageAltText = "R" },
                new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "Yellow", Image = "https://via.placeholder.com/20/FFFF00?text=Y", ImageAltText = "Y" },
                new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue", Image = "https://via.placeholder.com/20/0000FF?text=B", ImageAltText = "B"   },
            },
      };


      await turnContext.SendActivityAsync(reply, cancellationToken);
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
        var qm = result.Entities.SingleOrDefault(s => s.Type == "Quantity math") ?? result.Entities.SingleOrDefault(s => s.Type == "Measure Quantity");

        var inum = result.Entities.SingleOrDefault(s => s.Type == "ItemNumber");
        if (qm == null || inum == null)
        {
          dialogState[turnContext.Activity.Recipient.Id] = true;
          await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
        else
        {
          int q = getNumberInString(qm.Entity);
          int inu = getNumberInString(inum.Entity);
          await turnContext.SendActivityAsync(MessageFactory.Text($"q:{q} inu:{inu}"));
          //db.Select(SQL_Database.sql_cmd_select_tabItem);
        }

      }
      else if (topIntent == "Sell")
      {

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
  }
}
