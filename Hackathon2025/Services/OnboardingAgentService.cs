using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI;
using System.ClientModel;
using System.Text.Json;
using Azure;

namespace Hackathon2025.Abstractions
{
    public class OnboardingAgentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly VectorStoreService _vectorStoreService;


        public OnboardingAgentService(HttpClient httpClient, IConfiguration configuration, VectorStoreService vectorStoreService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _vectorStoreService = vectorStoreService;
        }
    }
}
