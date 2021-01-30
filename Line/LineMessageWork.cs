using Newtonsoft.Json.Linq;
using RestSharp;
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
        private readonly string PushMessageUrl = "https://api.line.me/v2/bot/message/multicast";

        public LineMsgBot()
        {
            
        }

        public async Task<HttpStatusCode> PushMessage(IList<string> userId, string sendMessage)
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.MyToken}");
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

        public string GetlineImage(string messageID)
        {
            string LineAPI = @$"https://api-data.line.me/v2/bot/message/{messageID}/content";

            var client = new RestClient(LineAPI);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {MyToken}");
            IRestResponse response = client.Execute(request);
            var result = response.RawBytes;

            return Imgur.Imgur.Upload(result);
        }

        public async Task<HttpStatusCode> PushJson(IList<string> userId, JObject msg)
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.MyToken}");
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
