using Newtonsoft.Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    class Collections
    {
        public async Task LineMsg(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var channelData = turnContext.Activity.ChannelData;
            var jsonObj = JsonConvert.SerializeObject(channelData);

            string msgType = jsonObj["type"];
            switch (msgType)
            {
                case "location":
                    await turnContext.SendActivityAsync(MessageFactory.Text(jsonObj["address"]), cancellationToken);
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