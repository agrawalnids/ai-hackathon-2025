using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;
using System.Text.Json;
using ChromaDB.Client;
using System.Net.Http;
using System;
using LangChain.Providers;

namespace Hackathon2025.Abstractions
{
    public class VectorStoreService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VectorStoreService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<string>> QueryOnboardingResultVectorStoreAsync(string message)
        {
            var endpoint = _configuration["GithubOpenAI:AzureAIEndpoint"];
            var credential = _configuration["GithubOpenAI:Token"];
            var model = _configuration["GithubOpenAI:AzureEmbeddingModel"];


            var configOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            var client = new ChromaClient(configOptions, _httpClient);

            var openAIOptions2 = new OpenAIClientOptions()
            {
                Endpoint = new Uri(endpoint)

            };

            var client2 = new EmbeddingClient(model, new ApiKeyCredential(credential), openAIOptions2);

            OpenAIEmbedding response = await client2.GenerateEmbeddingAsync(message);

            ReadOnlyMemory<float> vector1 = response.ToFloats();

            var collection = await client.GetOrCreateCollection("kitkat");
            var collectionClient = new ChromaCollectionClient(collection, configOptions, _httpClient);

            List<ReadOnlyMemory<float>> queryEmbedding = [vector1];

            var queryResult = await collectionClient.Query(
                queryEmbeddings: queryEmbedding,
                nResults: 2,
                include: ChromaQueryInclude.Metadatas | ChromaQueryInclude.Distances);

            List<string> resultLists = new List<string>();

            foreach (var result in queryResult)
            {
                foreach (var item in result)
                {
                    resultLists.Add((string)item.Metadata["Raw"]);
                }
            }

            return resultLists;
        }

        public async Task SaveOnboardingResultToVectorStoreAsync(List<OnboardingDataModel> onboardingDataList)
        {
            var endpoint = _configuration["GithubOpenAI:AzureAIEndpoint"];
            var credential = _configuration["GithubOpenAI:Token"];
            var model = _configuration["GithubOpenAI:AzureEmbeddingModel"];


            var configOptions = new ChromaConfigurationOptions(uri: "http://localhost:8000/api/v1/");
            var client = new ChromaClient(configOptions, _httpClient);

            var openAIOptions2 = new OpenAIClientOptions()
            {
                Endpoint = new Uri(endpoint)

            };

            var client2 = new EmbeddingClient(model, new ApiKeyCredential(credential), openAIOptions2);



            List<string> messageIds = new List<string>();
            List<ReadOnlyMemory<float>> messageEmbeddings = new List<ReadOnlyMemory<float>>();
            List<Dictionary<string, object>> metadata = new List<Dictionary<string, object>>();

            foreach (var item in onboardingDataList)
            {
                OpenAIEmbedding response = await client2.GenerateEmbeddingAsync(item.Data);
                ReadOnlyMemory<float> vector = response.ToFloats();
                messageEmbeddings.Add(vector);
                messageIds.Add(Guid.NewGuid().ToString());
                metadata.Add(item.MetaData);
            }


            var collection = await client.GetOrCreateCollection("kitkat");
            var collectionClient = new ChromaCollectionClient(collection, configOptions, _httpClient);

            await collectionClient.Add(messageIds, messageEmbeddings, metadata);
        }
    }
}
