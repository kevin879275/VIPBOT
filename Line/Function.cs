using Newtonsoft.Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class LineFunctions
    {
        public async Task AcceptedLineMsg(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var channelData = turnContext.Activity.ChannelData;
            string msgType = channelData["type"];
            switch (msgType)
            {
                case "location":
                    await turnContext.SendActivityAsync(MessageFactory.Text(channelData["address"]), cancellationToken);
                    string dataStr = JsonConvert.SerializeObject(channelData);
                    break;
                case "text":
                    break;
                case "image":
                    break;
                default:
                    break;
            }
        }

        public JObject setCard(string url, string itemName, string itemNum, string price, string description, string location, int itemID)
        {
            string raw = "{\"type\":\"bubble\",\"direction\":\"ltr\",\"hero\":{\"type\":\"image\",\"url\":\"\",\"align\":\"center\",\"size\":\"full\",\"aspectRatio\":\"20:13\",\"aspectMode\":\"cover\",\"backgroundColor\":\"#FFFFFFFF\"},\"body\":{\"type\":\"box\",\"layout\":\"vertical\",\"spacing\":\"sm\",\"contents\":[{\"type\":\"text\",\"text\":\"\u5929\u7afa\u9f20\u8eca\u8eca\",\"weight\":\"bold\",\"size\":\"xxl\",\"wrap\":true,\"contents\":[]},{\"type\":\"text\",\"text\":\"\u6578\u91cf:1\",\"weight\":\"bold\",\"size\":\"lg\",\"contents\":[]},{\"type\":\"text\",\"text\":\"$0\",\"weight\":\"bold\",\"contents\":[]},{\"type\":\"text\",\"text\":\"9\u6210\u65b0\",\"contents\":[]},{\"type\":\"text\",\"text\":\"loaction\",\"align\":\"start\",\"gravity\":\"top\",\"contents\":[]}]},\"footer\":{\"type\":\"box\",\"layout\":\"vertical\",\"spacing\":\"sm\",\"contents\":[{\"type\":\"button\",\"action\":{\"type\":\"message\",\"label\":\"\u4e0b\u55ae\",\"text\":\"aa\"},\"style\":\"primary\",\"gravity\":\"center\"},{\"type\":\"button\",\"action\":{\"type\":\"message\",\"label\":\"\u8a62\u554f\",\"text\":\"buy\"}}]}}";
            Card tmp = JsonConvert.DeserializeObject<Card>(raw);

            tmp.hero.url = url;
            tmp.body.contents[0].text = itemName;
            tmp.body.contents[1].text = itemNum;
            tmp.body.contents[2].text = price;
            tmp.body.contents[3].text = description;
            tmp.body.contents[4].text = location;
            tmp.footer.contents[0].text = "買 " + "#" + itemID.ToString();

            return JObject.FromObject(tmp);

        }
    }

}