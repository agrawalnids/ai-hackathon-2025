using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;
using System.Text.Json;
using ChromaDB.Client;
using System.Net.Http;
using System;
using LangChain.Providers;
using Hackathon2025.Services;
using Microsoft.Extensions.Configuration;

namespace Hackathon2025.Abstractions
{
    public class VectorStoreService
    {
        private readonly HttpClient _httpClient;
        static readonly GithubModelsConfig Config = ConfigurationHelper.GithubModelsConfig;

        public VectorStoreService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Check if the URL returned by chat model is valid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> IsUrlValid(string url)
        {
            using HttpClient client = new();
            try
            {
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get results from vector DB
        /// </summary>
        /// <param name="message">semantic query</param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> QueryOnboardingResultVectorStoreAsync(string message)
        {
            //locally deployed
            var chromaOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            var client = new ChromaClient(chromaOptions, _httpClient);

            //embedding options
            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(Config.AzureAIEndpoint)
            };

            var embeddingClient = new EmbeddingClient(Config.AzureEmbeddingModel, new ApiKeyCredential(Config.Token), openAIOptions);

            OpenAIEmbedding response = await embeddingClient.GenerateEmbeddingAsync(message);


            //convert response to vector
            ReadOnlyMemory<float> vector = response.ToFloats();

            List<ReadOnlyMemory<float>> queryEmbedding = [vector];

            var collection = await client.GetOrCreateCollection("kitkat");
            var collectionClient = new ChromaCollectionClient(collection, chromaOptions, _httpClient);

            //query embedding
            var queryResult = await collectionClient.Query(
                queryEmbeddings: queryEmbedding,
                nResults: 5,
                include: ChromaQueryInclude.Metadatas | ChromaQueryInclude.Distances);


            //Get results
            Dictionary<string, object> resultLists = new Dictionary<string, object>();

            foreach (var result in queryResult)
            {
                foreach (var item in result)
                {
                    resultLists.Add((string)item.Metadata["Label"], (string)item.Metadata["Description"]);
                }
            }

            return resultLists;
        }


        /// <summary>
        /// Save vectors to DB
        /// </summary>
        /// <param name="vectorDataList"></param>
        /// <returns></returns>
        public async Task SaveToVectorStoreAsync(List<VectorDataModel> vectorDataList)
        {

            var chromaOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            var client = new ChromaClient(chromaOptions, _httpClient);

            //embedding options
            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(Config.AzureAIEndpoint)
            };

            var embeddingClient = new EmbeddingClient(Config.AzureEmbeddingModel, new ApiKeyCredential(Config.Token), openAIOptions);


            List<string> messageIds = new List<string>();
            List<ReadOnlyMemory<float>> messageEmbeddings = new List<ReadOnlyMemory<float>>();
            List<Dictionary<string, object>> metadata = new List<Dictionary<string, object>>();

            //generate a vector list for every item
            foreach (var item in vectorDataList)
            {
                OpenAIEmbedding response = await embeddingClient.GenerateEmbeddingAsync(item.Data);
                ReadOnlyMemory<float> vector = response.ToFloats();
                messageEmbeddings.Add(vector);
                messageIds.Add(Guid.NewGuid().ToString());
                metadata.Add(item.MetaData);
            }


            var collection = await client.GetOrCreateCollection("kitkat");
            var collectionClient = new ChromaCollectionClient(collection, chromaOptions, _httpClient);

            await collectionClient.Add(messageIds, messageEmbeddings, metadata);
        }
    }
}
