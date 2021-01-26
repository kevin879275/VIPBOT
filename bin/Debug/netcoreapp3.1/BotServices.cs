//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License.

//using Microsoft.Bot.Builder.AI.QnA;
//using Microsoft.Extensions.Configuration;

//namespace Microsoft.BotBuilderSamples
//{
//    public class BotServices : IBotServices
//    {
//        public BotServices(IConfiguration configuration)
//        {
//            QnAMakerService = new QnAMaker(new QnAMakerEndpoint
//            {
//                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
//                EndpointKey = configuration["QnAAuthKey"],
//                Host = GetHostname(configuration["QnAEndpointHostName"])
//            });
//        }

//        public QnAMaker QnAMakerService { get; private set; }

//        private static string GetHostname(string hostname)
//        {
//            if (!hostname.StartsWith("https://"))
//            {
//                hostname = string.Concat("https://", hostname);
//            }

//            if (!hostname.EndsWith("/qnamaker") && !hostname.Contains("/v5.0"))
//            {
//                hostname = string.Concat(hostname, "/qnamaker");
//            }

//            return hostname;
//        }
//    }
//}
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            // If includeApiResults is set to true, the full response from the LUIS api (LuisResult)
            // will be made available in the properties collection of the RecognizerResult

            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
              configuration["LuisAPIHostName"]);

            // Set the recognizer options depending on which endpoint version you want to use.
            // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            Dispatch = new LuisRecognizer(recognizerOptions);

            SampleQnA = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAAuthKey"],
                Host = configuration["QnAEndpointHostName"]
            });

        }

        public LuisRecognizer Dispatch { get; private set; }
        public QnAMaker SampleQnA { get; private set; }
    }
}
