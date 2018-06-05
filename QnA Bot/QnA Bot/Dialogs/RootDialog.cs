using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using QnA_Bot.Classes;
using System;
using System.Net;
using System.Threading.Tasks;

namespace QnA_Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            string responseString = string.Empty;
            var query = activity.Text.ToString();

            string host = "";
            string EndpointKey = "";
            string knowledgeBase = ""; 

            var builder = new UriBuilder($"{host}/qnamaker/knowledgebases/{knowledgeBase}/generateAnswer");
            var postBody = $"{{\"question\": \"{query}\"}}";

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Authorization", EndpointKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }
            var receivedAnswer = QnAHelper.HandleResponse(responseString);
            await context.PostAsync(receivedAnswer);
            context.Wait(MessageReceivedAsync);
        }
    }
}