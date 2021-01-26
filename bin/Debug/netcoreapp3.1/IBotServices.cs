// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.Luis;


namespace Microsoft.BotBuilderSamples
{
    public interface IBotServices
    {
        LuisRecognizer Dispatch { get; }
        QnAMaker SampleQnA { get; }
    }
}
