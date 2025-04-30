using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI;
using System.ClientModel;
using System.Text.Json;
using Azure;
using MoodMate.Services;
using System.Net;

namespace MoodMate.Abstractions
{
    public class InterestsAgentService
    {
        public ChatClient? Client { get; set; }
        public ChatCompletionOptions? Options { get; set; }
        static readonly GithubModelsConfig Config = ConfigurationHelper.GithubModelsConfig;


        /// <summary>
        /// Connects to Github Open AI Chat Completion model
        /// </summary>
        /// <param name="jsonSchema"></param>
        /// <param name="schemaName"></param>
        /// <param name="chatHistory">chat so far</param>
        /// <returns></returns>
        public async Task<ChatCompletion> GetChatCompletionResponse(string jsonSchema, string schemaName, List<ChatMessage> chatHistory)
        {
            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(Config.ChatEndpoint)
            };

            Client = new ChatClient(Config.ChatModel, new ApiKeyCredential(Config.Token), openAIOptions);

            Options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: schemaName,
                    jsonSchema: BinaryData.FromString(jsonSchema),
                    jsonSchemaIsStrict: true)
            };

            return await Client.CompleteChatAsync(chatHistory, Options);
        }
    }
}
