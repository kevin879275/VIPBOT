﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.BotBuilderSamples
{
    public class LineMsgBot
    {
        private readonly string MyToken = "vdwJ5U7YzRTT89PRsT+m13HgHBvuzoZ5boR5Pu662LfWyuiYHiheFYRYsTmeuLqTRXF5VZPOeQsL7NZr82JnswuEo96RtbdAnvzzvstyXRNrpUJkgLbkgfB9ydVmayyXTy3albpIqx4pxA2mc5TD3QdB04t89/1O/w1cDnyilFU=";
        private readonly string PushMessageUrl = "https://api.line.me/v2/bot/message/push";
        private HttpClient Client = new HttpClient();

        public LineMsgBot()
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.MyToken}");
        }

        public async Task<HttpStatusCode> PushMessage(IList<string> userId, string sendMessage)
        {
            try
            {
                var post = await Client.PostAsJsonAsync(PushMessageUrl, new
                {
                    to = userId,
                    messages = new[] {
                    new {
                        type="text",
                        text =sendMessage,
                        }
                    }
                });

                return post.StatusCode;
            }
            catch (Exception x)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(x.Message) });
            }
        }

        public async Task<HttpStatusCode> PushJson(IList<string> userId, JObject msg)
        {
            try
            {
                var post = await Client.PostAsJsonAsync(PushMessageUrl, new
                {
                    to = userId,
                    messages = msg
                });

                return post.StatusCode;
            }
            catch (Exception x)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(x.Message) });
            }
        }
    }
}
