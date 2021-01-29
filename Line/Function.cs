using Newtonsoft.Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

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

    }

}