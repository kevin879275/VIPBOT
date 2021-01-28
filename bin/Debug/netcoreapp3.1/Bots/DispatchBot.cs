using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Imgur;
namespace Microsoft.BotBuilderSamples
{
    public class DispatchBot : ActivityHandler
    {
        private readonly ILogger<DispatchBot> _logger;
        private readonly IBotServices _botServices;

        private static readonly HttpClient client = new HttpClient();

        private readonly string[] _cards = {
            //Path.Combine (".", "Cards", "Covid19Status.json"),
            //Path.Combine (".", "Cards", "GlobalStatus.json"),
        };

        public DispatchBot(IBotServices botServices, ILogger<DispatchBot> logger)
        {
            _logger = logger;
            _botServices = botServices;
            
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if(turnContext.Activity.Attachments.Count==1)
            {
                string s=Imgur.UploadSrc(turnContext.Activity.Attachments[0].ContentUrl);
                turnContext.SendActivityAsync(MessageFactory.Text(s), cancellationToken);
            }
            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
            // Top intent tell us which cognitive service to use.
            var topIntent = recognizerResult.GetTopScoringIntent();

            // Next, we call the dispatcher with the top intent.
            await DispatchToTopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

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
            
            //try
            //{
            //    SQL_Database sQL_Database = new SQL_Database();
            //    var cb = new SqlConnectionStringBuilder
            //    {
            //        DataSource = "viplabcareerhackserver.database.windows.net",
            //        UserID = "viplab",
            //        Password = "Careerhack12345",
            //        InitialCatalog = "VIPLABCAREERHACKDB",
            //    };
            //        using (var connection = new SqlConnection(cb.ConnectionString))
            //    {
            //        connection.Open();
            //        sQL_Database.Submit_Tsql_NonQuery(connection, "Create-Tables", sQL_Database.sql_cmd_CreateTables());

            //        //sQL_Database.Submit_Tsql_Insert_tabUser(connection, "Insert into table user",
            //        //    sQL_Database.sql_cmd_Insert_tabUser(), "Taiwan", "dog");



            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
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

        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
        {
            switch (intent)
            {
                case "l_BuySell":
                    //var result = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(recognizerResult.Properties["luisResult"], new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    //var topIntent = result.Prediction.Intents[result.Prediction.TopIntent];
                    await ProcessCovid19LuisAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                // case "l_Weather":
                //     await ProcessWeatherAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                //     break;
                case "q_BuySell":
                    await ProcessSampleQnAAsync(turnContext, cancellationToken);
                    break;
                default:
                    _logger.LogInformation($"機器人無法辨識您");
                    await turnContext.SendActivityAsync(MessageFactory.Text($"機器人無法辨識您"), cancellationToken);
                    break;
            }
        }

        private async Task ProcessCovid19LuisAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessBuySellLuisAsync");

            // Retrieve LUIS result for Process Automation.
            var result = luisResult.ConnectedServiceResult;
            var topIntent = result.TopScoringIntent.Intent;
            if(topIntent == "Number")
            {

            }
            else if(topIntent == "Buy")
            {

            }
            else if(topIntent == "Sell")
            {

            }
            //if (topIntent == "Covid19India")
            //{
            //    var url = $"https://api.covid19api.com/total/country/india";
            //    var repositories = ProcessRepo(url);
            //    List<string> Ystdata = new List<string>();
            //    List<string> Tdydata = new List<string>();

            //    if (repositories.Count > 0)
            //    {
            //        var ystData = repositories[repositories.Count - 2];
            //        var todayData = repositories.LastOrDefault();

            //        DateTime Todaydatetime = DateTime.Parse(todayData.date);
            //        DateTime Ystdatetime = DateTime.Parse(ystData.date);

            //        var confirmedInc = todayData.confirmed - ystData.confirmed;
            //        var confirmedIncPCT = (((Math.Abs(Convert.ToDecimal(confirmedInc))) / (Math.Abs(Convert.ToDecimal(todayData.confirmed)))) * 100).ToString("0.00");

            //        var activeInc = todayData.active - ystData.active;
            //        var activeIncPCT = (((Math.Abs(Convert.ToDecimal(activeInc))) / (Math.Abs(Convert.ToDecimal(todayData.active)))) * 100).ToString("0.00"); //(activeInc / todayData.active) * 100;

            //        var recoveredInc = todayData.recovered - ystData.recovered;
            //        var recoveredIncPCT = (((Math.Abs(Convert.ToDecimal(recoveredInc))) / (Math.Abs(Convert.ToDecimal(todayData.recovered)))) * 100).ToString("0.00"); //(recoveredInc / todayData.recovered) * 100;

            //        var deceasedInc = todayData.deaths - ystData.deaths;
            //        var deceasedIncPCT = (((Math.Abs(Convert.ToDecimal(deceasedInc))) / (Math.Abs(Convert.ToDecimal(todayData.deaths)))) * 100).ToString("0.00"); //(deceasedInc / todayData.deaths) * 100;

            //        Ystdata.Add(ystData.confirmed.ToString());
            //        Ystdata.Add(ystData.deaths.ToString());
            //        Ystdata.Add(ystData.recovered.ToString());
            //        Ystdata.Add(ystData.active.ToString());
            //        Ystdata.Add(Ystdatetime.ToString("dd MMM yyyy"));

            //        Tdydata.Add(todayData.confirmed.ToString());
            //        Tdydata.Add(todayData.deaths.ToString());
            //        Tdydata.Add(todayData.recovered.ToString());
            //        Tdydata.Add(todayData.active.ToString());
            //        Tdydata.Add(Todaydatetime.ToString("dd MMM yyyy") + " " + "India");
            //        Tdydata.Add("▲ " + " " + confirmedInc + " " + "(" + confirmedIncPCT + " " + "%" + ")");
            //        Tdydata.Add("▲ " + " " + activeInc + " " + "(" + activeIncPCT + " " + "%" + ")");
            //        Tdydata.Add("▲ " + " " + recoveredInc + " " + "(" + recoveredIncPCT + " " + "%" + ")");
            //        Tdydata.Add("▲ " + " " + deceasedInc + " " + "(" + deceasedIncPCT + " " + "%" + ")");
            //    }

            //    var Covid19StatusCardRead = readFileforUpdate_jobj(_cards[0]);

            //    JToken Date = Covid19StatusCardRead.SelectToken("body[0].items[2].text");
            //    JToken ConfirmedInc = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[0].items[1].text");
            //    JToken Confirmed = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[0].items[2].text");

            //    JToken ActiveInc = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[1].items[1].text");
            //    JToken Active = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[1].items[2].text");

            //    JToken RecoveredInc = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[2].items[1].text");
            //    JToken Recovered = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[2].items[2].text");

            //    JToken DeceasedInc = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[3].items[1].text");
            //    JToken Deceased = Covid19StatusCardRead.SelectToken("body[0].items[3].columns[3].items[2].text");

            //    Date.Replace(Tdydata[4]);
            //    Confirmed.Replace(Tdydata[0]);
            //    Active.Replace(Tdydata[3]);
            //    Recovered.Replace(Tdydata[2]);
            //    Deceased.Replace(Tdydata[1]);

            //    ConfirmedInc.Replace(Tdydata[5]);
            //    ActiveInc.Replace(Tdydata[6]);
            //    RecoveredInc.Replace(Tdydata[7]);
            //    DeceasedInc.Replace(Tdydata[8]);

            //    var Covid19StatusCardFinal = UpdateAdaptivecardAttachment(Covid19StatusCardRead);
            //    var response = MessageFactory.Attachment(Covid19StatusCardFinal, ssml: "Covid19 Live status card!");
            //    await turnContext.SendActivityAsync(response, cancellationToken);
            //}
            //else
            //if (topIntent == "WorldCovid19")
            //{
            //    var url = $"https://api.covid19api.com/summary";
            //    var repositories = ProcessRepoWorldJ(url);
            //    List<string> Tdydata = new List<string>();

            //    var TotalConfirmed = (string)repositories.SelectToken("Global.TotalConfirmed");
            //    var NewConfirmed = (string)repositories.SelectToken("Global.NewConfirmed");
            //    var TotalDeaths = (string)repositories.SelectToken("Global.TotalDeaths");
            //    var NewDeaths = (string)repositories.SelectToken("Global.NewDeaths");
            //    var TotalRecovered = (string)repositories.SelectToken("Global.TotalRecovered");
            //    var NewRecovered = (string)repositories.SelectToken("Global.NewRecovered");
            //    var date = (DateTime)repositories.SelectToken("Date");
            //    //DateTime datetime1 = DateTime.Parse(date);
            //    //DateTime datetime1 = DateTime.Parse(date);
            //    var Todaydatetime = date.ToString("dd MMM yyyy") + " " + "Global";
            //    var confirmedIncPCT = (((Math.Abs(Convert.ToDecimal(NewConfirmed))) / (Math.Abs(Convert.ToDecimal(TotalConfirmed)))) * 100).ToString("0.00");
            //    var recoveredIncPCT = (((Math.Abs(Convert.ToDecimal(NewRecovered))) / (Math.Abs(Convert.ToDecimal(TotalRecovered)))) * 100).ToString("0.00");
            //    var deathsIncPCT = (((Math.Abs(Convert.ToDecimal(NewDeaths))) / (Math.Abs(Convert.ToDecimal(TotalDeaths)))) * 100).ToString("0.00");

            //    var confirmIncFinal = "▲ " + " " + NewConfirmed + " " + "(" + confirmedIncPCT + " " + "%" + ")";
            //    var recoveredFinal = "▲ " + " " + NewRecovered + " " + "(" + recoveredIncPCT + " " + "%" + ")";
            //    var deathsIncFinal = "▲ " + " " + NewDeaths + " " + "(" + deathsIncPCT + " " + "%" + ")";

            //    var GlobalStatusCardRead = readFileforUpdate_jobj(_cards[1]);

            //    JToken Date = GlobalStatusCardRead.SelectToken("body[0].items[2].text");
            //    JToken ConfirmedInc = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[0].items[1].text");
            //    JToken Confirmed = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[0].items[2].text");

            //    JToken RecoveredInc = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[1].items[1].text");
            //    JToken Recovered = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[1].items[2].text");

            //    JToken DeceasedInc = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[2].items[1].text");
            //    JToken Deceased = GlobalStatusCardRead.SelectToken("body[0].items[3].columns[2].items[2].text");

            //    Date.Replace(Todaydatetime);
            //    ConfirmedInc.Replace(confirmIncFinal);
            //    Confirmed.Replace(TotalConfirmed);
            //    RecoveredInc.Replace(recoveredFinal);
            //    Recovered.Replace(TotalRecovered);
            //    DeceasedInc.Replace(deathsIncFinal);
            //    Deceased.Replace(TotalDeaths);
            //    //var GlobalStatusCardFinal = CreateAdaptiveCardAttachment(_cards[1]);
            //    var GlobalStatusCardFinal = UpdateAdaptivecardAttachment(GlobalStatusCardRead);
            //    var response = MessageFactory.Attachment(GlobalStatusCardFinal, ssml: "Global Live status card!");
            //    await turnContext.SendActivityAsync(response, cancellationToken);
            //}
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
